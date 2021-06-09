using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Units;
using System;
using System.Net.Http;
using System.Threading;
using System.Text.Json;
using System.Threading.Tasks;
using MeadowTemperature.Mcu.Data;
using System.Net;

namespace MeadowTemperature.Mcu
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.1.5:5000/") };
        AnalogTemperature temperatureSensor;

        public MeadowApp()
        {
            InitializeTemperatureSensor();
            InitializeNetworkingAsync().Wait();
            InitializeClockAsync().Wait();            
        }

        private void InitializeTemperatureSensor()
        {
            temperatureSensor = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A01,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );
            temperatureSensor.TemperatureUpdated += TemperatureUpdated;
            temperatureSensor.StartUpdating(standbyDuration: 5000);
        }

        private async Task InitializeNetworkingAsync()
        {
            if (!Device.InitWiFiAdapter().Result)
            {
                throw new Exception("Could not initialize the WiFi adapter.");
            }

            var connectionResult = await Device.WiFiAdapter.Connect("SSID", "password");
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }                       
        }

        private async Task InitializeClockAsync()
        {
            Device.SetClock(await GetDateTimeFromApi());
        }

        public async Task<HttpResponseMessage> UploadTemperatureDataAsync(TemperatureData temperatureData)
        {
            string jsonTemperature = JsonSerializer.Serialize(temperatureData);
            var stringContent = new StringContent(jsonTemperature, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await httpClient.PostAsync($"api/temperature", stringContent);
            }
            catch(Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = $"{e.Message}";
            }
            return response;
        }

        public async Task<DateTime> GetDateTimeFromApi()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await client.GetAsync($"http://worldclockapi.com/api/json/utc/now");
                }
                catch (Exception e)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.ReasonPhrase = $"{e.Message}";
                }               

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ClockApiResponse>(responseContent);
                    Console.WriteLine(apiResponse.currentDateTime);
                    return DateTime.Parse(apiResponse.currentDateTime);
                }
                
                return new DateTime();
            }
        }

        private async void TemperatureUpdated(object sender, IChangeResult<Temperature> e)
        {
            Console.WriteLine(e.New.Celsius);

            TemperatureData temperatureData = new TemperatureData { DateTime = DateTime.Now, TemperatureC = Math.Round(e.New.Celsius,1) };

            await UploadTemperatureDataAsync(temperatureData);
        }
    }
}

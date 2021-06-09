using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeadowTemperature.Mcu.Data
{
    public class TemperatureData
    {
        public DateTime DateTime { get; set; }
        public double TemperatureC { get; set; }
    }
}

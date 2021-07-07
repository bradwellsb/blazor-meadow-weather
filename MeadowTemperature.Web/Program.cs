using MeadowTemperature.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeadowTemperature.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var sp = host.Services.GetService<IServiceScopeFactory>()
               .CreateScope()
               .ServiceProvider;
            var options = sp.GetRequiredService<DbContextOptions<TemperatureDataContext>>();

            await CreateDbIfNotExistsAsync(options);
            host.Run();
        }

        private static async Task CreateDbIfNotExistsAsync(DbContextOptions<TemperatureDataContext> options)
        {
            var builder = new DbContextOptionsBuilder<TemperatureDataContext>(options);

            using var context = new TemperatureDataContext(builder.Options);

            await context.Database.EnsureCreatedAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

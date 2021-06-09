using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeadowTemperature.Web.Data
{
    public class TemperatureDataContext : DbContext
    {
        public TemperatureDataContext(DbContextOptions<TemperatureDataContext> options)
            : base(options)
        {
        }

        public DbSet<TemperatureData> TemperatureData { get; set; }

    }
}

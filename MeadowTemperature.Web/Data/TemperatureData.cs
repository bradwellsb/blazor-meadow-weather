using System;
using System.ComponentModel.DataAnnotations;

namespace MeadowTemperature.Web.Data
{
    public class TemperatureData
    {
        [Key]
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public double TemperatureC { get; set; }

    }
}

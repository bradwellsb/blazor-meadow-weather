using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeadowTemperature.Web.Data;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace MeadowTemperature.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly TemperatureDataContext _context;

        public TemperatureController(IDbContextFactory<TemperatureDataContext> dbFactory)
        {
            _context = dbFactory.CreateDbContext();
        }

        // GET: api/Temperature/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureData>> GetTemperatureById(long id)
        {
            var temperatureItem = await _context.TemperatureData.FindAsync(id);

            if (temperatureItem == null)
            {
                return NotFound();
            }

            return temperatureItem;
        }

        // POST: api/Temperature
        [HttpPost]
        public async Task<ActionResult<TemperatureData>> PostTemperatureData(TemperatureData temperatureData)
        {
            _context.TemperatureData.Add(temperatureData);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemperatureById), new { id = temperatureData.Id }, temperatureData);
        }
    }
}

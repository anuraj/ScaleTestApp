using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ScaleTestApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet, Route("api/cpu-load")]
        public IActionResult CpuLoad(int seconds, int percentage)
        {
            FakeWorkload(seconds, percentage);
 
            return Ok($"Fake load of {percentage}% for {seconds} seconds");
        }

        private void FakeWorkload(int seconds, int percentageCpu)
        {
            percentageCpu = Math.Max(100, percentageCpu);
            var end = DateTime.Now.AddSeconds(seconds);
            var percentage = percentageCpu;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (DateTime.Now < end)
            {
                // Make the loop go on for "percentage" milliseconds then sleep the
                // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                if (watch.ElapsedMilliseconds > percentage)
                {
                    System.Threading.Thread.Sleep(100 - percentage);
                    watch.Reset();
                    watch.Start();
                }
            }
        }
    }
}

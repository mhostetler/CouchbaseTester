using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CouchbaseTester.Controllers
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
        private readonly IClusterProvider _clusterProvider;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IClusterProvider clusterProvider)
        {
            _logger = logger;
            _clusterProvider = clusterProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var cluster = await _clusterProvider.GetClusterAsync();
            var result = await cluster.QueryAsync<dynamic>("select gs.* from `gamesim-sample` AS gs where jsonType = \"player\" LIMIT 10;");
            var resultRows = await result.Rows.ToListAsync();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                //Summary = Summaries[rng.Next(Summaries.Length)],
                Summary = resultRows[rng.Next(resultRows.Count())].name
            })
            .ToArray();
        }
    }
}

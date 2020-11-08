using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Couchbase.Linq;
using Couchbase;

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
        private readonly Task<IBucket> _bucketTask;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Task<IBucket> bucketTask)
        {
            _logger = logger;
            _bucketTask = bucketTask;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            IList<Player> resultRows = new Player[] { new Player() { Name = "undefined"} };
            try
            {
                var bucketContext = new BucketContext(await _bucketTask);
                resultRows = bucketContext
                    .Query<Player>()
                    .Where(x => x.JsonType == "player")
                    .Take(20)
                    .ToList();                
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error querying Couchbase");
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                //Summary = Summaries[rng.Next(Summaries.Length)],
                Summary = resultRows[rng.Next(resultRows.Count())].Name
            })
            .ToArray();
        }
    }
}

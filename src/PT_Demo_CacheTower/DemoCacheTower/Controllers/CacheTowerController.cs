using CacheTower;
using DemoCacheTower.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoCacheTower.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheTowerController : ControllerBase
    {
        private readonly ILogger<CacheTowerController> _logger;
        private readonly ICacheStack<UserContext> _cacheStack;

        public CacheTowerController(
            ILogger<CacheTowerController> logger,
            ICacheStack<UserContext> cacheStack)
        {
            _logger = logger;
            _cacheStack = cacheStack;
        }

        [HttpGet("MemoryCache/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _cacheStack.GetOrSetAsync<UserProfile>($"user-{id}", async (old, context) =>
            {
                return await context.GetUserForIdAsync(id);
            }, new CacheSettings(TimeSpan.FromDays(1), TimeSpan.FromMinutes(60)));

            return Ok(result);
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
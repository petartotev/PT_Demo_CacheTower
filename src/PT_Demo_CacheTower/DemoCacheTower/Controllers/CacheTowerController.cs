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

        [HttpGet("GetOrSet/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _cacheStack.GetOrSetAsync<UserProfile>($"user-{id}", async (old, context) =>
            {
                return await context.GetUserForIdAsync(id);
            }, new CacheSettings(TimeSpan.FromDays(1), TimeSpan.FromMinutes(60)));

            return Ok(result);
        }
    }
}
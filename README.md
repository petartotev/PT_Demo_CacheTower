# PT_Demo_CacheTower

[Cache Tower](https://github.com/TurnerSoftware/CacheTower) isn't a single type of cache, its a multi-layer solution to caching with each layer on top of another.<br>A multi-layer cache provides the performance benefits of a fast cache like in-memory with the resilience of a file, database or Redis-backed cache.

## Contents
- [Prerequisites](#prerequisites)
- [CacheTower](#cachetower)
    - [MemoryCache](#memorycache)
- [Links](#links)

## Prerequisites

1. Create new blank .NET Solution `PT_Demo_CacheTower` and add new .NET 6 Web API project `DemoCacheTower`.

2. Introduce new classes `UserProfile.cs` and `UserContext.cs`:

```
using ProtoBuf;

namespace DemoCacheTower.Models;

[ProtoContract]
public class UserProfile
{
    [ProtoMember(1)]
    public int UserId { get; set; }
    [ProtoMember(2)]
    public string UserName { get; set; }
    [ProtoMember(3)]
    public DateTime DateCreatedOrUpdated { get; set; }
}
```

```
public class UserContext
{
    public async Task<UserProfile> GetUserForIdAsync(int id)
    {
        return new UserProfile
        {
            UserId = id,
            UserName = $"username-{id}",
            DateCreatedOrUpdated = DateTime.Now
        };
    }
}
```

3. Add `UserContext` to container in `Program.cs`:

```
            builder.Services.AddSingleton<UserContext>();
```

4. Install the following NuGet packages:
- `CacheTower`
- `CacheTower.Providers.Redis`

## CacheTower

### MemoryCache

5. AddCacheStack in `Program.cs`:

```
            builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
                .AddMemoryCacheLayer()
                //.AddRedisCacheLayer(/* Your Redis Connection */, new RedisCacheLayerOptions(ProtobufCacheSerializer.Instance))
                .WithCleanupFrequency(TimeSpan.FromMinutes(5)));
```

6. Create new CacheTowerController.cs:

```
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
    }
}
```

## Links
- https://github.com/TurnerSoftware/CacheTower
- https://www.nuget.org/packages/CacheTower
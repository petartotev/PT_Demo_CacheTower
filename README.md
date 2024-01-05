# PT_Demo_CacheTower

[Cache Tower](https://github.com/TurnerSoftware/CacheTower) isn't a single type of cache, its a multi-layer solution to caching with each layer on top of another.<br>A multi-layer cache provides the performance benefits of a fast cache like in-memory with the resilience of a file, database or Redis-backed cache.

## Contents
- [Prerequisites](#prerequisites)
- [CacheTower](#cachetower)
    - [MemoryCache](#memorycache)
    - [File-based Caching](#file-based-caching)
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

0. Do steps from [Prerequisites](#prerequisites) section.

1. AddCacheStack in `Program.cs`:

```
            builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
                .AddMemoryCacheLayer()
                //.AddRedisCacheLayer(/* Your Redis Connection */, new RedisCacheLayerOptions(ProtobufCacheSerializer.Instance))
                .WithCleanupFrequency(TimeSpan.FromMinutes(5)));
```

2. Create new CacheTowerController.cs:

```
using DemoCacheTower.Models;

namespace DemoCacheTower;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<UserContext>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // =============== 1) MemoryCache ===============
        builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
            .AddMemoryCacheLayer()
            .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
```

3. Test

### File-based Caching

*Provides a basic file-based caching solution using your choice of serializer. It stores each serialized cache item into its own file and uses a singular manifest file to track the status of the cache.*

![FileCache](./res/cachetower_filecache_01.png)

0. Do steps from [MemoryCache](#memorycache) section.

1. Install Serializer:

```
Install-Package CacheTower.Serializers.NewtonsoftJson
```

2. Update CacheTowerController.cs as follows:

```
using CacheTower.Providers.FileSystem;
using CacheTower.Serializers.NewtonsoftJson;
using DemoCacheTower.Models;

namespace DemoCacheTower;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<UserContext>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // =============== 2) FileCache ===============
        builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
            .AddFileCacheLayer(new FileCacheLayerOptions("~/", NewtonsoftJsonCacheSerializer.Instance))
            .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
```

3. Test

## Links
- https://github.com/TurnerSoftware/CacheTower
- https://www.nuget.org/packages/CacheTower
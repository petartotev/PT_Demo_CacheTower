using CacheTower.Providers.FileSystem;
using CacheTower.Providers.Redis;
using CacheTower.Serializers.NewtonsoftJson;
using CacheTower.Serializers.Protobuf;
using DemoCacheTower.Models;
using MongoDB.Driver;
using MongoFramework;
using ProtoBuf.Meta;
using StackExchange.Redis;

namespace DemoCacheTower;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSingleton<UserContext>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // =============== 1) MemoryCache ===============
        //builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
        //    .AddMemoryCacheLayer()
        //    .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        // =============== 2) FileCache ===============
        //builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
        //    .AddFileCacheLayer(new FileCacheLayerOptions("~/", NewtonsoftJsonCacheSerializer.Instance))
        //    .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        // =============== 3) RedisCache ===============
        //builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
        //    .AddRedisCacheLayer(
        //        ConnectionMultiplexer.Connect("127.0.0.1:6379"),
        //        new RedisCacheLayerOptions(ProtobufCacheSerializer.Instance))
        //    .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        // =============== 4) MongoDbCache ===============
        builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
            .AddMongoDbCacheLayer(MongoDbConnection.FromConnectionString("mongodb://localhost:27017/TestDb"))
            .WithCleanupFrequency(TimeSpan.FromMinutes(5)));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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
using CacheTower.Providers.Redis;
using CacheTower.Serializers.Protobuf;
using DemoCacheTower.Models;
using ProtoBuf.Meta;

namespace DemoCacheTower
{
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

            builder.Services.AddCacheStack<UserContext>((provider, builder) => builder
                .AddMemoryCacheLayer()
                //.AddRedisCacheLayer(/* Your Redis Connection */, new RedisCacheLayerOptions(ProtobufCacheSerializer.Instance))
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
}
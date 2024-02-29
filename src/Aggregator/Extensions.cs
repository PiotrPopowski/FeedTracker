using FeedTracker.Aggregator.Weather;
using FeedTracker.Shared.Logging;
using FeedTracker.Shared.Messaging;
using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Redis;
using FeedTracker.Shared.Serialization;
using MassTransit;

namespace FeedTracker.Aggregator
{
    internal static class Extensions
    {
        internal static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            builder.Host.AddSerilog();
            builder.Logging.UseOpenTelemetry(builder.Environment.ApplicationName);
            
            return builder;
        }

        internal static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();
            
            return builder;
        }

        internal static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddSingleton<IWeatherHandler, WeatherHandler>()
                .AddHostedService<WeatherBackgroundService>();

            return builder;
        }

        internal static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOpenTelemerty(builder.Environment.ApplicationName)
                .AddRedisStreaming(new RedisOptions() { ConnectionString = builder.Configuration["Redis:ConnectionString"] })
                .AddSerializer()
                .AddMassTransit(mt =>
                {
                    mt.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(builder.Configuration["Messaging:Host"]);
                    });
                })
                .AddMessaging(builder.Configuration);

            return builder;
        }
    }
}

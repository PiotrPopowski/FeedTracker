using FeedTracker.Feeds.Weather;
using FeedTracker.Shared.Logging;
using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Redis;
using FeedTracker.Shared.Serialization;
using Quartz;

namespace FeedTracker.Feeds
{
    internal static class Extensions
    {
        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            builder.Host.AddSerilog();
            builder.Logging.UseOpenTelemetry(builder.Environment.ApplicationName);

            return builder;
        }

        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            return builder;
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddBackgroundServices()
                .AddHttpClient<WeatherClient>();

            return builder;
        }

        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOpenTelemerty(builder.Environment.ApplicationName)
                .AddSerializer()
                .AddRedisStreaming(new RedisOptions() { ConnectionString = builder.Configuration["Redis:ConnectionString"] });

            return builder;
        }

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services
                .AddScoped<WeatherBackgroundService>()
                .ConfigureOptions<WeatherBackgroundServiceSetup>()
                .AddQuartz()
                .AddQuartzHostedService(opt =>
                {
                    opt.WaitForJobsToComplete = true;
                });

            return services;
        }
    }
}

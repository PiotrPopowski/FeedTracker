using FeedTracker.Shared.Redis.Streaming;
using FeedTracker.Shared.Streaming;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FeedTracker.Shared.Redis
{
    public static class Extensions
    {
        public static IServiceCollection AddRedisStreaming(this IServiceCollection services, RedisOptions options)
            => services
                .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString))
                .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
                .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
    }
}

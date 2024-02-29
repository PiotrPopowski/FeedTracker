using Microsoft.Extensions.DependencyInjection;

namespace FeedTracker.Shared.Serialization
{
    public static class Extensions
    {
        public static IServiceCollection AddSerializer(this IServiceCollection services)
            => services.AddSingleton<ISerializer, SystemTextJsonSerializer>();
    }
}

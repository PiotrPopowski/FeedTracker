using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FeedTracker.Shared.Messaging
{
    public static class Extensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            services.AddSingleton<IMessagePublisher, MessagePublisher>();

            return services;
        }
    }
}

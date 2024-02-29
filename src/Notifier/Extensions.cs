using FeedTracker.Notifier.Common;
using FeedTracker.Shared.Logging;
using FeedTracker.Shared.Messaging;
using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Serialization;
using FeedTracker.Subscribers.Protos;
using MassTransit;

namespace FeedTracker.Notifier
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
            builder.Services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(); 
            
            return builder;
        }

        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IEmailService, EmailService>();

            return builder;
        }

        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOpenTelemerty(builder.Environment.ApplicationName)
                .AddMassTransit(mt => 
                {
                    mt.AddConsumers(typeof(Program).Assembly);
                    mt.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(builder.Configuration["Messaging:Host"]);
                        cfg.ConfigureEndpoints(ctx);
                    });
                })
                .AddMessaging(builder.Configuration)
                .AddSerializer()
                .AddGrpcClient<SubscriberService.SubscriberServiceClient>(opt 
                    => opt.Address = new Uri(builder.Configuration.GetValue<string>("GrpcSettings:SubscribersConnection")));

            return builder;
        }
    }
}

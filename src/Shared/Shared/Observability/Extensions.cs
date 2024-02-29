using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FeedTracker.Shared.Observability
{
    public static class Extensions
    {
        private static string correlationKey = "CorrelationId";

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
            => app.Use(async (ctx, next) =>
            {
                if (!ctx.Request.Headers.TryGetValue(correlationKey, out var correlationId))
                {
                    correlationId = Guid.NewGuid().ToString("N");
                }
                ctx.Items.TryAdd(correlationKey, correlationId);

                await next();
            });

        public static IServiceCollection AddOpenTelemerty(this IServiceCollection services, string serviceName)
        {
            ActivitySource activitySource = new(serviceName);

            services.AddOpenTelemetry()
                    .ConfigureResource(r => r.AddService(serviceName))
                    .WithTracing(t => t
                        .AddSource(activitySource.Name)
                        .AddGrpcClientInstrumentation()
                        .AddAspNetCoreInstrumentation(opt => opt.Filter = excludeMetrics)
                        .AddOtlpExporter(o => o.Endpoint = new Uri($"http://jaeger:4317")));

            services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
            services.AddSingleton(activitySource);
            services.AddSingleton<IActivityContextAccessor, ActivityContextAccessor>();
            services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
            services.AddHttpContextAccessor();

            return services;

            bool excludeMetrics(HttpContext context) => !context.Request.Path.Value?.StartsWith("/metrics") ?? true;
        }

        public static string? GetCorrelationId(this HttpContext context)
            => context.Items.TryGetValue(correlationKey, out var correlationId) ? correlationId as string : null;
    }
}

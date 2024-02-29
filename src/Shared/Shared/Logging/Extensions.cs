using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace FeedTracker.Shared.Logging
{
    public static class Extensions
    {
        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
            => hostBuilder.UseSerilog((ctx, config) =>
            {
                config
                    .WriteTo.Seq(ctx.Configuration["Seq:Host"])
                    .WriteTo.Console()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .Filter.ByExcluding(metrics)
                    .Enrich.WithExceptionDetails();
            });

        public static ILoggingBuilder UseOpenTelemetry(this ILoggingBuilder builder, string serviceName)
            => builder.AddOpenTelemetry(opt => opt
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName))
                .AddOtlpExporter(o => o.Endpoint = new Uri($"http://jaeger:4317")));

        private static bool metrics(LogEvent e)
        {
            e.Properties.TryGetValue("Path", out var path);
            return path?.ToString().StartsWith("\"/metrics") ?? false;
        }
    }
}

using FeedTracker.Shared.Streaming;
using OpenTelemetry.Trace;
using Quartz;
using System.Diagnostics;

namespace FeedTracker.Feeds.Weather
{
    [DisallowConcurrentExecution]
    public class WeatherBackgroundService : IJob
    {
        private readonly IStreamPublisher _streamPublisher;
        private readonly ILogger<WeatherBackgroundService> _logger;
        private readonly WeatherClient _weatherClient;
        private readonly Tracer _tracer;

        public WeatherBackgroundService(IStreamPublisher streamPublisher, ILogger<WeatherBackgroundService> logger, 
            WeatherClient weatherClient, Tracer tracer)
        {
            _streamPublisher = streamPublisher;
            _logger = logger;
            _weatherClient = weatherClient;
            _tracer = tracer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var span = _tracer.StartActiveSpan("tracing weather");
            span.SetAttribute("traceId", span.Context.TraceId.ToString());

            var weather = await _weatherClient.GetAsync("Poznan");

            var correlationId = Guid.NewGuid().ToString();
            _logger.LogInformation("Feeding weather: {WeatherData}. CorrelationId: {CorrelationId}", weather, correlationId);

            await _streamPublisher.PublishAsync("weather", weather, correlationId);
        }
    }
}

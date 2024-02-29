using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Streaming;
using Prometheus;
using System.Diagnostics;

namespace FeedTracker.Aggregator.Weather
{
    public class WeatherBackgroundService : BackgroundService
    {
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly IWeatherHandler _weatherHandler;
        private readonly ILogger<WeatherBackgroundService> _logger;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private static readonly Counter TotalWeatherEvents = Metrics.CreateCounter("weather_total_events", "Number of total messages processed.");

        public WeatherBackgroundService(IStreamSubscriber streamSubscriber, IWeatherHandler weatherHandler, 
            ILogger<WeatherBackgroundService> logger, ICorrelationIdAccessor correlationIdAccessor)
        {
            _streamSubscriber = streamSubscriber;
            _logger = logger;
            this._correlationIdAccessor = correlationIdAccessor;
            _weatherHandler = weatherHandler;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<WeatherData>("weather",
                (weatherData, ctx) =>
                {
                    _logger.LogInformation("Aggregating weather. CorrelationId: {CorrelationId}.", ctx.CorrelationId);
                    _correlationIdAccessor.SetCorrelationId(ctx.CorrelationId);

                    _weatherHandler.HandleAsync(weatherData);
                    
                    Activity.Current?.AddEvent(new ActivityEvent("Aggregated weather messages."));
                    
                    TotalWeatherEvents.Inc(1);
                });
        }
    }
}

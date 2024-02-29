using FeedTracker.Aggregator.Weather.Rules;
using FeedTracker.Contracts;
using FeedTracker.Contracts.Weather;
using FeedTracker.Shared.Messaging;
using FeedTracker.Shared.Observability;

namespace FeedTracker.Aggregator.Weather
{
    public class WeatherHandler : IWeatherHandler
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;

        public WeatherHandler(IMessagePublisher messagePublisher, ICorrelationIdAccessor correlationIdAccessor)
        {
            this._messagePublisher = messagePublisher;
            this._correlationIdAccessor = correlationIdAccessor;
        }

        public async Task HandleAsync(WeatherData weatherData)
        {
            if(new IsHighTempPolicy(weatherData, DateTime.Now).IsApplicable())
            {
               await _messagePublisher.PublishAsync(new HighTemperatureMessage(weatherData.TempC, DateTime.Now), _correlationIdAccessor.CorrelationId);
            }
        }
    }
}

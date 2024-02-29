using FeedTracker.Contracts;

namespace FeedTracker.Aggregator.Weather
{
    public interface IWeatherHandler
    {
        Task HandleAsync(WeatherData weatherData);
    }
}
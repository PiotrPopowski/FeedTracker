namespace FeedTracker.Aggregator.Weather.Rules
{
    public class IsWindyPolicy : IPolicy
    {
        private readonly WeatherData _weatherData;

        public IsWindyPolicy(WeatherData weatherData)
        {
            this._weatherData = weatherData;
        }

        public bool IsApplicable()
        {
            return _weatherData.WindMps >= 12;
        }
    }
}

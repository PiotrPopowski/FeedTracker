namespace FeedTracker.Aggregator.Weather.Rules
{
    public class IsHighTempPolicy : IPolicy
    {
        private readonly WeatherData _weatherData;
        private readonly DateTime _now;

        public IsHighTempPolicy(WeatherData weatherData, DateTime now)
        {
            _weatherData = weatherData;
            _now = now;
        }

        public bool IsApplicable()
        {
            var maxTemp = _now.Date.Month switch
            {
                >= 2 and <= 4 => 2, //spring
                >= 5 and <= 7 => 3, // summer
                >= 8 and <= 10 => 4, // autumn
                _ => 1 // winter
            };

            return _weatherData.TempC >= maxTemp;
        }
    }
}

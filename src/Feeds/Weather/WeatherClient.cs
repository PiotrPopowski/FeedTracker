using OpenTelemetry.Trace;

namespace FeedTracker.Feeds.Weather
{
    public sealed class WeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly Tracer tracer;

        public WeatherClient(HttpClient httpClient, Tracer tracer)
        {
            _httpClient = httpClient;
            this.tracer = tracer;
        }

        public async Task<WeatherData> GetAsync(string city)
        {
            //var weather = await _httpClient.GetFromJsonAsync<WeatherData>("");
            using var span = tracer.StartActiveSpan("fetching weather via http request.");
            var random = new Random();
            var weather = new WeatherData() { TempC = random.Next(-1, 18), WindMps = random.Next(0, 8) };
            return await Task.FromResult(weather);
        }
    }
}

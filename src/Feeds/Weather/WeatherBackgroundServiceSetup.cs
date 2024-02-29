using Microsoft.Extensions.Options;
using Quartz;

namespace FeedTracker.Feeds.Weather
{
    public class WeatherBackgroundServiceSetup : IConfigureOptions<QuartzOptions>
    {
        private readonly int _interval;

        public WeatherBackgroundServiceSetup(IConfiguration configuration)
        {
            _interval = configuration.GetValue<int>("WeatherBackgroundService:IntervalInSeconds");
        }

        public void Configure(QuartzOptions options)
        {
            var jobKey = nameof(WeatherBackgroundService);
            options.AddJob<WeatherBackgroundService>(jobBuilder => jobBuilder.WithIdentity(jobKey))
               .AddTrigger(trigger => trigger
                    .ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(_interval)
                        .RepeatForever()));
        }
    }
}

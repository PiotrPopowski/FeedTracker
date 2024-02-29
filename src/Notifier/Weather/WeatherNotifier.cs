using FeedTracker.Contracts.Weather;
using FeedTracker.Shared.Messaging;
using FeedTracker.Shared.Serialization;
using FeedTracker.Subscribers.Protos;
using Grpc.Core;
using MassTransit;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Text.Json;

namespace FeedTracker.Notifier.Weather
{
    public class WeatherNotifier : IConsumer<HighTemperatureMessage>
    {
        private readonly SubscriberService.SubscriberServiceClient _subscriberService;
        private readonly ILogger _logger;
        private readonly Tracer _tracer;
        private readonly ActivitySource _activitySource;
        private readonly ISerializer _serializer;

        public WeatherNotifier(SubscriberService.SubscriberServiceClient subscriberService, ILogger<WeatherNotifier> logger, 
            Tracer tracer, ActivitySource activitySource, ISerializer serializer)
        {
            _tracer = tracer;
            this._activitySource = activitySource;
            this._serializer = serializer;
            _logger = logger;
            _subscriberService = subscriberService;
        }

        public async Task Consume(ConsumeContext<HighTemperatureMessage> context)
        {
            var parentLink = new List<ActivityLink>();
            ActivityContext? activityContext = null;
            if (context.TryGetHeader<string>("activity-context", out var serializedActivity))
            {
                activityContext = _serializer.Deserialize<ActivityContext>(serializedActivity);
                parentLink.Add(new ActivityLink(activityContext.Value));
            }
            using var span = _activitySource.StartActivity($"consuming-HighTemperatureMessage", ActivityKind.Consumer, activityContext ?? default, null, parentLink);

            var correlationHeader = new Metadata
            {
                { "CorrelationId", context.CorrelationId.ToString() ?? string.Empty }
            };
            var subscribers = await _subscriberService.GetSubscribersAsync(new GetSubscribersRequest(), new CallOptions().WithHeaders(correlationHeader));
            foreach (var subscriber in subscribers.Emails)
            {
                _logger.LogInformation("Notifing {Subscriber}. Message {Message}. CorrelationId: {CorrelationId}. MessageId: {MessageId}.",
                    subscriber,
                    JsonSerializer.Serialize(context.Message),
                    context.CorrelationId,
                    context.MessageId);
            }
        }
    }
}

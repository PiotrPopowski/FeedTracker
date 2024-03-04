using FeedTracker.Contracts.Weather;
using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Observability.Utilities;
using FeedTracker.Subscribers.Protos;
using Grpc.Core;
using MassTransit;
using System.Text.Json;

namespace FeedTracker.Notifier.Weather
{
    public class WeatherNotifier : IConsumer<HighTemperatureMessage>
    {
        private readonly SubscriberService.SubscriberServiceClient _subscriberService;
        private readonly ILogger _logger;

        public WeatherNotifier(SubscriberService.SubscriberServiceClient subscriberService, ILogger<WeatherNotifier> logger)
        {
            _logger = logger;
            _subscriberService = subscriberService;
        }

        public async Task Consume(ConsumeContext<HighTemperatureMessage> context)
        {
            using var span = DiagnosticsConfig.Source.StartActivityFromPropagationContext(
                DiagnosticNames.ConsumingMessage<HighTemperatureMessage>(),
                context,
                (ctx, key) => ctx.Headers.TryGetHeader(key, out var value) ? new List<string> { ((string)value) } : Enumerable.Empty<string>());

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

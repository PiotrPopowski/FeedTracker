using FeedTracker.Shared.Serialization;
using FeedTracker.Shared.Streaming;
using StackExchange.Redis;
using FeedTracker.Shared.Observability.Utilities;
using FeedTracker.Shared.Observability;

namespace FeedTracker.Shared.Redis.Streaming
{
    internal class RedisStreamSubscriber : IStreamSubscriber
    {
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;

        public RedisStreamSubscriber(IConnectionMultiplexer multiplexer, ISerializer serializer)
        {
            _subscriber = multiplexer.GetSubscriber();
            _serializer = serializer;
        }

        public Task SubscribeAsync<T>(string topic, Action<T, StreamContext<T>> handler) where T : class
        {
            return _subscriber.SubscribeAsync(RedisChannel.Literal(topic), (_, data) =>
            {
                var payload = _serializer.Deserialize<StreamContext<T>>(data);
                if (payload is null)
                    return;

                using var span = DiagnosticsConfig.Source
                    .StartActivityFromPropagationContext(DiagnosticNames.ProcessingMessage<T>(), payload, ExtractContextFromStream);

                handler(payload.Message, payload);
            });
        }

        private IEnumerable<string> ExtractContextFromStream<T>(StreamContext<T> context, string key) where T : class
            => context.ApplicationProperties.TryGetValue(key, out var value) ? new List<string> { value } : Enumerable.Empty<string>();
    }
}

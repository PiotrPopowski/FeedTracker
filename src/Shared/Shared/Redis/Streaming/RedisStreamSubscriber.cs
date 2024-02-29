using FeedTracker.Shared.Serialization;
using FeedTracker.Shared.Streaming;
using StackExchange.Redis;
using System.Diagnostics;

namespace FeedTracker.Shared.Redis.Streaming
{
    internal class RedisStreamSubscriber : IStreamSubscriber
    {
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;
        private readonly ActivitySource _activitySource;

        public RedisStreamSubscriber(IConnectionMultiplexer multiplexer, ISerializer serializer, ActivitySource activitySource)
        {
            _subscriber = multiplexer.GetSubscriber();
            _serializer = serializer;
            _activitySource = activitySource;
        }

        public Task SubscribeAsync<T>(string topic, Action<T, StreamContext<T>> handler) where T : class
        {
            return _subscriber.SubscribeAsync(RedisChannel.Literal(topic), (_, data) =>
            {
                var payload = _serializer.Deserialize<StreamContext<T>>(data);
                if (payload is null)
                    return;

                var parentLink = new List<ActivityLink>();
                if(payload.Context.HasValue) parentLink.Add(new ActivityLink(payload.Context.Value));

                using var span = _activitySource.StartActivity($"processing-{typeof(T).Name}", ActivityKind.Consumer, payload.Context ?? default, null, parentLink);

                handler(payload.Message, payload);
            });
        }
    }
}

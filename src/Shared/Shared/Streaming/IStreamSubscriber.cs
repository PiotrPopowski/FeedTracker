namespace FeedTracker.Shared.Streaming;

public interface IStreamSubscriber
{
    Task SubscribeAsync<T>(string topic, Action<T, StreamContext<T>> handler) where T: class;
}

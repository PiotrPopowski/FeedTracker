namespace FeedTracker.Shared.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string? correlationId = null) where T : class;
    }
}

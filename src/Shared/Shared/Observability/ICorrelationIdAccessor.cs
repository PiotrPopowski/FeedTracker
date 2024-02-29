namespace FeedTracker.Shared.Observability
{
    public interface ICorrelationIdAccessor
    {
        string? CorrelationId { get; }
        void SetCorrelationId(string? correlationId);
    }
}

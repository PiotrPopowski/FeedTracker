namespace FeedTracker.Shared.Streaming
{
    public record StreamContext<T> where T : class
    {
        public T Message { get; init; } 
        public string CorrelationId { get; init; }
        public Dictionary<string, string> ApplicationProperties { get; init; }

        public StreamContext(T Message, string CorrelationId)
        {
            ApplicationProperties = new Dictionary<string, string>();
            this.Message = Message;
            this.CorrelationId = CorrelationId;
        }
    }

}

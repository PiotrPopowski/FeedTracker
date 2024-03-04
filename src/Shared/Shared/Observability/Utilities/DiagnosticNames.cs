
namespace FeedTracker.Shared.Observability.Utilities
{
    public static class DiagnosticNames
    {
        public static string ProcessingMessage<T>() => $"processing-{typeof(T).Name}";

        public static string StreamingMessage<T>() => $"streaming-{typeof(T).Name}";

        public static string PublishingMessage<T>() => $"publishing-{typeof(T).Name}";

        public static string ConsumingMessage<T>() => $"consuming-{typeof(T).Name}";
    }
}

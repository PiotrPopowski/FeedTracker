using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FeedTracker.Shared.Streaming
{
    public record StreamContext<T>(T Message, string CorrelationId, ActivityContext? Context) where T : class;
}

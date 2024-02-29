using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FeedTracker.Shared.Streaming;

public interface IStreamPublisher
{
    Task PublishAsync<T>(string topic,  T data, string? correlationId = null) where T : class;
}

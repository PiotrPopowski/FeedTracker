using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Serialization;
using FeedTracker.Shared.Streaming;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using StackExchange.Redis;
using System.Diagnostics;

namespace FeedTracker.Shared.Redis.Streaming;

internal sealed class RedisStreamPublisher : IStreamPublisher
{
    private readonly ISubscriber _subscriber;
    private readonly ISerializer _serializer;
    private readonly ActivitySource _activitySource;
    private readonly IActivityContextAccessor _activityContextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RedisStreamPublisher(IConnectionMultiplexer multiplexer, IHttpContextAccessor httpContextAccessor, ISerializer serializer, 
        ActivitySource activitySource, IActivityContextAccessor activityContextAccessor)
    {
        _subscriber = multiplexer.GetSubscriber();
        _serializer = serializer;
        this._activitySource = activitySource;
        this._activityContextAccessor = activityContextAccessor;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task PublishAsync<T>(string topic, T data, string? correlationId = null) where T : class
    {
        using var span = _activitySource.StartActivity($"streaming-{typeof(T).Name}", ActivityKind.Producer, _activityContextAccessor.Current ?? default);
        span.SetTag("traceId", _activityContextAccessor.Parent.Value.TraceId.ToString());

        if (string.IsNullOrEmpty(correlationId))
            correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId() ?? Guid.NewGuid().ToString();

        var envelope = new StreamContext<T>(data, correlationId, _activityContextAccessor.Parent);
        var payload = _serializer.Serialize(envelope);
        
        return _subscriber.PublishAsync(RedisChannel.Literal(topic), payload);
    }
}

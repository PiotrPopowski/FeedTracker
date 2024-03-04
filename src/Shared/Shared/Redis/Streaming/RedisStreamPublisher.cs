using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Observability.Utilities;
using FeedTracker.Shared.Serialization;
using FeedTracker.Shared.Streaming;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace FeedTracker.Shared.Redis.Streaming;

internal sealed class RedisStreamPublisher : IStreamPublisher
{
    private readonly ISubscriber _subscriber;
    private readonly ISerializer _serializer;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RedisStreamPublisher(IConnectionMultiplexer multiplexer, IHttpContextAccessor httpContextAccessor, ISerializer serializer)
    {
        _subscriber = multiplexer.GetSubscriber();
        _serializer = serializer;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task PublishAsync<T>(string topic, T data, string? correlationId = null) where T : class
    {
        if (string.IsNullOrEmpty(correlationId))
            correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId() ?? Guid.NewGuid().ToString();

        var envelope = new StreamContext<T>(data, correlationId);

        using var span = DiagnosticsConfig.Source.StartActivityWithPropagation(
            DiagnosticNames.StreamingMessage<T>(),
            envelope,
            (ctx, key, value) => ctx.ApplicationProperties[key] = value);

        var payload = _serializer.Serialize(envelope);
        return _subscriber.PublishAsync(RedisChannel.Literal(topic), payload);
    }
}

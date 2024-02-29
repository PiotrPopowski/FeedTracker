using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace FeedTracker.Shared.Messaging
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivitySource _activitySource;
        private readonly IActivityContextAccessor _activityContextAccessor;
        private readonly ISerializer _serializer;

        public MessagePublisher(IBus bus, IHttpContextAccessor httpContextAccessor, ActivitySource activitySource, 
            IActivityContextAccessor activityContextAccessor, ISerializer serializer)
        {
            _bus = bus;
            this._httpContextAccessor = httpContextAccessor;
            this._activitySource = activitySource;
            this._activityContextAccessor = activityContextAccessor;
            _serializer = serializer;
        }

        public async Task PublishAsync<T>(T message, string? correlationId = null) where T : class
        {
            using var span = _activitySource.StartActivity($"publishing-{typeof(T).Name}", ActivityKind.Producer, _activityContextAccessor.Current ?? default);

            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.TryParse(_httpContextAccessor.HttpContext?.GetCorrelationId(), out var correlationContextId) 
                    ? correlationContextId.ToString()
                    : Guid.NewGuid().ToString();
            }

            await _bus.Publish(message, ctx => 
            { 
                ctx.CorrelationId = Guid.Parse(correlationId);
                ctx.Headers.Set("activity-context", _serializer.Serialize(_activityContextAccessor.Parent));
            });
        }
    }
}

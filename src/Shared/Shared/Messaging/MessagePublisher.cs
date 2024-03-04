using FeedTracker.Shared.Observability;
using FeedTracker.Shared.Observability.Utilities;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace FeedTracker.Shared.Messaging
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessagePublisher(IBus bus, IHttpContextAccessor httpContextAccessor)
        {
            _bus = bus;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task PublishAsync<T>(T message, string? correlationId = null) where T : class
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.TryParse(_httpContextAccessor.HttpContext?.GetCorrelationId(), out var correlationContextId) 
                    ? correlationContextId.ToString()
                    : Guid.NewGuid().ToString();
            }

            Dictionary<string, string?> headers = new();
            DiagnosticsConfig.Source.StartActivityWithPropagation(
                DiagnosticNames.PublishingMessage<T>(),
                headers,
                (headers, key, value) => headers.Add(key, value));

            await _bus.Publish(message, ctx => 
            { 
                ctx.CorrelationId = Guid.Parse(correlationId);
                foreach(var header in headers)
                    ctx.Headers.Set(header.Key, header.Value);
            });
        }
    }
}

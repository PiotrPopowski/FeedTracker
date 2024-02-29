using Microsoft.AspNetCore.Http;

namespace FeedTracker.Shared.Observability
{
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        private static readonly AsyncLocal<string?> _correlationId = new AsyncLocal<string?>();

        public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if(string.IsNullOrEmpty(_correlationId.Value))
                _correlationId.Value = httpContextAccessor.HttpContext?.GetCorrelationId();
        }

        public string? CorrelationId => _correlationId.Value;

        public void SetCorrelationId(string? correlationId)
        {
            _correlationId.Value = correlationId;
        }
    }
}

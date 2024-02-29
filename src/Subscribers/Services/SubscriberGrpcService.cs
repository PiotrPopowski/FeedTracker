using FeedTracker.Subscribers.Protos;
using Grpc.Core;

namespace FeedTracker.Subscribers.Services
{
    public class SubscriberGrpcService : Protos.SubscriberService.SubscriberServiceBase
    {
        private readonly ISubscriberService _subscriberService;
        private readonly ILogger<SubscriberGrpcService> _logger;

        public SubscriberGrpcService(ISubscriberService subscriberService, ILogger<SubscriberGrpcService> logger)
        {
            _subscriberService = subscriberService;
            this._logger = logger;
        }

        public override async Task<GetSubscribersResponse> GetSubscribers(GetSubscribersRequest request, ServerCallContext context)
        {
            var correlationHeader = new Metadata.Entry("CorrelationId", context.RequestHeaders.GetValue("CorrelationId"));
            await context.WriteResponseHeadersAsync(new Metadata() { correlationHeader });
            _logger.LogInformation("Getting Subscribers. CorrelationId: {CorrelationId}", correlationHeader.Value);

            var response = new GetSubscribersResponse();
            var subscribers = _subscriberService.GetAll();
            response.Emails.AddRange(subscribers.Select(x => x.Email));

            return response;
        }
    }
}

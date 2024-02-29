
namespace FeedTracker.Notifier.Common
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }


        public Task Send(string email, string message)
        {
            _logger.LogInformation("Email sent to {0} with message {1}.", email, message);
            return Task.CompletedTask;
        }
    }
}

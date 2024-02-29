namespace FeedTracker.Notifier.Common
{
    public interface IEmailService
    {
        Task Send(string email, string message);
    }
}

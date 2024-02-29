namespace FeedTracker.Notifier.Common.External
{
    public class Subscriber
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public Subscriber() { }

        public Subscriber(Guid id, string email)
        {
            Id = id;
            Email = email;
        }
    }
}

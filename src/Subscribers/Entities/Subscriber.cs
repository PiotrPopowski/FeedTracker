namespace FeedTracker.Subscribers.Entities
{
    public class Subscriber
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        private Subscriber() { }

        public Subscriber(string email)
        {
            Id = Guid.NewGuid();
            Email = email;
        }
    }
}

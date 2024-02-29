using FeedTracker.Subscribers.Entities;

namespace FeedTracker.Subscribers.Services
{
    public class SubscriberService : ISubscriberService
    {
        private static List<Subscriber> _subscribers = new()
        {
            new Subscriber("popo@mytestmail.ie"),
            new Subscriber("customity@test.com")
        };

        public List<Subscriber> GetAll() => _subscribers;
    }
}

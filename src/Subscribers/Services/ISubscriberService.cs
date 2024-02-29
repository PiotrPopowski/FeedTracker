using FeedTracker.Subscribers.Entities;

namespace FeedTracker.Subscribers.Services
{
    public interface ISubscriberService
    {
        List<Subscriber> GetAll();
    }
}
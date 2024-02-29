using System.Diagnostics;

namespace FeedTracker.Shared.Observability
{
    internal interface IActivityContextAccessor
    {
        ActivityContext? Current { get; }
        ActivityContext? Parent { get; }
    }
}

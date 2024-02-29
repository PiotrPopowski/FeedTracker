using System.Diagnostics;

namespace FeedTracker.Shared.Observability
{
    internal class ActivityContextAccessor : IActivityContextAccessor
    {
        public ActivityContext? Current => Activity.Current?.Context;

        public ActivityContext? Parent => string.IsNullOrEmpty(Activity.Current?.ParentId)
            ? Activity.Current?.Context
            : new ActivityContext(Activity.Current.TraceId, Activity.Current.ParentSpanId, Activity.Current.ActivityTraceFlags);
    }
}

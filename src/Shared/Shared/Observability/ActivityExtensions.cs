using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;

namespace FeedTracker.Shared.Observability
{
    public static class ActivityExtensions
    {
        public static Activity? StartActivityWithPropagation<T>(this ActivitySource activitySource, string activityName,
            T carrier,
            Action<T, string, string> setter,
            IEnumerable<KeyValuePair<string, object?>>? tags = null)
        {
            Propagators.DefaultTextMapPropagator.Inject(
                new PropagationContext(Activity.Current?.Context ?? default, Baggage.Current),
                carrier,
                setter);

            return activitySource.StartActivity(activityName, ActivityKind.Producer, Activity.Current?.Context ?? default, tags);
        }

        public static Activity? StartActivityFromPropagationContext<T>(this ActivitySource activitySource, string activityName,
            T carrier,
            Func<T, string, IEnumerable<string>> getter,
            IEnumerable<KeyValuePair<string, object?>>? tags = null)
        {
            var propCtx = Propagators.DefaultTextMapPropagator.Extract(
                new PropagationContext(new ActivityContext(), Baggage.Current),
                carrier,
                getter);

            return activitySource.StartActivity(activityName, ActivityKind.Consumer, propCtx.ActivityContext, tags);
        }
    }
}
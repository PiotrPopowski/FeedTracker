using System.Diagnostics;

namespace FeedTracker.Shared.Observability.Utilities
{
    public static class DiagnosticsConfig
    {
        private static ActivitySource _source;
        public static ActivitySource Source 
        {
            get
            {
                if (_source is null) 
                    throw new Exception("Diagnostic source has not been configured.");
                return _source;
            }
            private set
            {
                if (value is null)
                    throw new Exception("Activity source cannot be null.");
                _source = value;
            }
        }

        internal static void Configure(ActivitySource source) => Source = source;
    }
}

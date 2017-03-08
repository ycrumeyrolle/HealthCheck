using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public abstract class ThresholdWatchSettings : WatchSettings
    {
        public ThresholdWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold)
            : base(name, critical, frequency, tags)
        {
            ErrorThreshold = errorThreshold;
            WarningThreshold = warningThreshold;
        }

        public long ErrorThreshold { get; set; }

        public long WarningThreshold { get; set; }

        public abstract bool ReachWarningThreshold(long value);

        public abstract bool ReachErrorThreshold(long value);
    }
}
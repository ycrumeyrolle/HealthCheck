using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class FloorThresholdWatchSettings : ThresholdWatchSettings
    {
        public FloorThresholdWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold)
            : base(name, critical, frequency, tags, errorThreshold, warningThreshold)
        {
        }

        public override bool ReachWarningThreshold(long value)
        {
            return value <= WarningThreshold;
        }

        public override bool ReachErrorThreshold(long value)
        {
            return value <= ErrorThreshold;
        }
    }
}
using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class CeilingThresholdCheckSettings : ThresholdCheckSettings
    {
        public CeilingThresholdCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold)
            : base(name, critical, frequency, tags, errorThreshold, warningThreshold)
        {
        }

        public override bool HasReachedWarningThreshold(long value)
        {
            return value >= WarningThreshold;
        }

        public override bool HasReachedErrorThreshold(long value)
        {
            return value >= ErrorThreshold;
        }
    }
}
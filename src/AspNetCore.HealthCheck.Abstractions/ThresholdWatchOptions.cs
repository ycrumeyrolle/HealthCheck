using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class ThresholdWatchOptions : WatchOptions
    {
        public long ErrorThreshold { get; set; }

        public long WarningThreshold { get; set; }
    }
}
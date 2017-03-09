using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class CounterWatchSettings : WatchSettings
    {
        public CounterWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long threshold, long warningThreshold, bool distributed)
            : base(name, critical, frequency, tags)
        {
            Threshold = threshold;
            WarningThreshold = WarningThreshold;
            Distributed = distributed;
        }

        public bool Distributed { get; set; }

        public long WarningThreshold { get; set; }

        public long Threshold { get; set; }
    }
}
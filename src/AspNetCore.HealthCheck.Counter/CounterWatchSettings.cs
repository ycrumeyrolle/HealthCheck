using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class CounterWatchSettings : WatchSettings
    {
        public CounterWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long threshold, bool distributed)
            : base(name, critical, frequency, tags)
        {
            Threshold = threshold;
            Distributed = distributed;
        }

        public bool Distributed { get; set; }

        public long Threshold { get; set; }
    }
}
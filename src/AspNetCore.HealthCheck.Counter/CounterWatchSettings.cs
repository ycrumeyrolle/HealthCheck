using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class CounterWatchSettings : CeilingThresholdWatchSettings
    {
        public CounterWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold, bool distributed)
            : base(name, critical, frequency, tags, errorThreshold, warningThreshold)
        {
            Distributed = distributed;
        }

        public bool Distributed { get; set; }
    }
}
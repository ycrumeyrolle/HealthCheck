using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class VirtualMemorySizeWatchSettings : WatchSettings
    {
        public VirtualMemorySizeWatchSettings(string name, int frequency, bool critical, long threshold, IEnumerable<string> tags)
            : base(name, critical, frequency, tags)
        {
            Threshold = threshold;
        }

        public long Threshold { get; }
    }
}
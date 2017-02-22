using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class AvailableDiskSpaceWatchSettings : WatchSettings
    {
        public AvailableDiskSpaceWatchSettings(string name, int frequency, bool critical, string drive, long threshold, IEnumerable<string> tags)
            : base(name, critical, frequency, tags)
        {
            Drive = drive;
            Threshold = threshold;
        }

        public string Drive { get; }

        public long Threshold { get; }
    }
}
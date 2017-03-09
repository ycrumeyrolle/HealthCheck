using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class AvailableDiskSpaceWatchSettings : FloorThresholdWatchSettings
    {
        public AvailableDiskSpaceWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold, string drive) 
            : base(name, critical, frequency, tags, errorThreshold, warningThreshold)
        {
            Drive = drive;
        }

        public string Drive { get; }
    }
}
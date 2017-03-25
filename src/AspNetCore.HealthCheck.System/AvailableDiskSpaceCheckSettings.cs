using System.Collections.Generic;

namespace AspNetCore.HealthCheck.System
{
    public class AvailableDiskSpaceCheckSettings : FloorThresholdCheckSettings
    {
        public AvailableDiskSpaceCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags, long errorThreshold, long warningThreshold, string drive) 
            : base(name, critical, frequency, tags, errorThreshold, warningThreshold)
        {
            Drive = drive;
        }

        public string Drive { get; }
    }
}
using System;

namespace AspNetCore.HealthCheck.System
{
    public class AvailableDiskSpaceCheckSettingsBuilder : ThresholdHealthCheckBuilder<AvailableDiskSpaceCheckSettings>
    {
        private string _drive;

        public AvailableDiskSpaceCheckSettingsBuilder(string name) 
            : base(name)
        {
            Tags.Add("system");
        }

        public AvailableDiskSpaceCheckSettingsBuilder WithDrive(string drive)
        {
            _drive = drive;
            return this;
        }

        public override AvailableDiskSpaceCheckSettings Build()
        {
            if (string.IsNullOrEmpty(_drive))
            {
                throw new InvalidOperationException("No drive were defined.");
            }

            return new AvailableDiskSpaceCheckSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold, _drive);
        }
    }
}

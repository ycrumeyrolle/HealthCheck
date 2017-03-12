using System;

namespace AspNetCore.HealthCheck.System
{
    public class AvailableDiskSpaceHealthCheckBuilder : ThresholdHealthCheckBuilder<AvailableDiskSpaceWatchSettings>
    {
        private string _drive;

        public AvailableDiskSpaceHealthCheckBuilder(string name) 
            : base(name)
        {
            Tags.Add("system");
        }

        public AvailableDiskSpaceHealthCheckBuilder WithDrive(string drive)
        {
            _drive = drive;
            return this;
        }

        public override AvailableDiskSpaceWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_drive))
            {
                throw new InvalidOperationException("No drive were defined.");
            }

            return new AvailableDiskSpaceWatchSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold, _drive);
        }
    }
}

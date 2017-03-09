namespace AspNetCore.HealthCheck.Process
{
    public class AvailableDiskSpaceHealthCheckBuilder : ThresholdHealthCheckBuilder<AvailableDiskSpaceWatchSettings>
    {
        private string _drive;

        public AvailableDiskSpaceHealthCheckBuilder(string name) 
            : base(name)
        {
        }

        public AvailableDiskSpaceHealthCheckBuilder WithDrive(string drive)
        {
            _drive = drive;
            return this;
        }

        public override AvailableDiskSpaceWatchSettings Build()
        {
            return new AvailableDiskSpaceWatchSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold, _drive);
        }
    }
}

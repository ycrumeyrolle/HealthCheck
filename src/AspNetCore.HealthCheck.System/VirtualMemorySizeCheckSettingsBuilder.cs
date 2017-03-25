namespace AspNetCore.HealthCheck.System
{
    public class VirtualMemorySizeCheckSettingsBuilder : ThresholdHealthCheckBuilder<FloorThresholdCheckSettings>
    {
        public VirtualMemorySizeCheckSettingsBuilder(string name) 
            : base(name)
        {
            Tags.Add("system");
        }

        public override FloorThresholdCheckSettings Build()
        {
            return new FloorThresholdCheckSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold);
        }
    }
}

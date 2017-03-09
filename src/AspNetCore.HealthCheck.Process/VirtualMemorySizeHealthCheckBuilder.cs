namespace AspNetCore.HealthCheck.Process
{
    public class VirtualMemorySizeHealthCheckBuilder : ThresholdHealthCheckBuilder<FloorThresholdWatchSettings>
    {
        public VirtualMemorySizeHealthCheckBuilder(string name) 
            : base(name)
        {
        }

        public override FloorThresholdWatchSettings Build()
        {
            return new FloorThresholdWatchSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold);
        }
    }
}

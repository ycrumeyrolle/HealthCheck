namespace AspNetCore.HealthCheck
{
    public class ThresholdCheckOptions : CheckOptions
    {
        public long ErrorThreshold { get; set; }

        public long WarningThreshold { get; set; }
    }
}
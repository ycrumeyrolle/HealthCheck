namespace AspNetCore.HealthCheck
{
    public class AvailableDiskSpaceOptions : ThresholdWatchOptions
    {
        public string Drive { get; set; }
    }
}
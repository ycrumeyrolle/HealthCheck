namespace AspNetCore.HealthCheck
{
    public class AvailableDiskSpaceCheckOptions : ThresholdCheckOptions
    {
        public string Drive { get; set; }
    }
}
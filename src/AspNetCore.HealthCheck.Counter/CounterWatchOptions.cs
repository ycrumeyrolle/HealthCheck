namespace AspNetCore.HealthCheck
{
    public class CounterWatchOptions : WatchOptions
    {
        public bool Distributed { get; set; }

        public long WarningThreshold { get; set; }

        public long Threshold { get; set; }
    }
}
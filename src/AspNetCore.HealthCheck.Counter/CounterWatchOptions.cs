namespace AspNetCore.HealthCheck
{
    public class CounterWatchOptions : ThresholdWatchOptions
    {
        public bool Distributed { get; set; }
    }
}
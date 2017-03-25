namespace AspNetCore.HealthCheck
{
    public class CounterCheckOptions : ThresholdCheckOptions
    {
        public bool Distributed { get; set; }
    }
}
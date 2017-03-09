namespace AspNetCore.HealthCheck.Counter
{
    public class CounterHealthCheckBuilder : ThresholdHealthCheckBuilder<CounterWatchSettings>
    {
        private bool _distributed;

        public CounterHealthCheckBuilder(string name)
            : base(name)
        {
        }
        
        public CounterHealthCheckBuilder IsDistributed()
        {
            _distributed = true;
            return this;
        }

        public CounterHealthCheckBuilder IsLocal()
        {
            _distributed = false;
            return this;
        }

        public override CounterWatchSettings Build()
        {
            return new CounterWatchSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold, _distributed);
        }
    }
}
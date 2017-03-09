namespace AspNetCore.HealthCheck.Counter
{
    public class CounterHealthCheckBuilder : SettingsHealthCheckBuilder<CounterWatchSettings>
    {
        private long _threshold;
        private long _warningThreshold;
        private bool _distributed;

        public CounterHealthCheckBuilder(string name)
            : base(name)
        {
        }
        
        public CounterHealthCheckBuilder WithThreshold(long threshold)
        {
            _threshold = threshold;
            return this;
        }

        public CounterHealthCheckBuilder WithWarningThreshold(long warningThreshold)
        {
            _warningThreshold = warningThreshold;
            return this;
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
            return new CounterWatchSettings(Name, Critical, Frequency, Tags, _threshold, _warningThreshold, _distributed);
        }
    }
}
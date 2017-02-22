namespace AspNetCore.HealthCheck.Counter
{
    public class CounterHealthCheckBuilder : SettingsHealthCheckBuilder<CounterWatchSettings>
    {
        private long _threshold;
        private bool _distributed;

        public CounterHealthCheckBuilder(string name)
            : base(name)
        {
        }

        public CounterHealthCheckBuilder WithUri(long threshold)
        {
            _threshold = threshold;
            return this;
        }

        public CounterHealthCheckBuilder WithThreshold(long threshold)
        {
            _threshold = threshold;
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
            return new CounterWatchSettings(Name, Critical, Frequency, Tags, _threshold, _distributed);
        }
    }
}
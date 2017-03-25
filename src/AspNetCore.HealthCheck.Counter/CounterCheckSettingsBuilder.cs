namespace AspNetCore.HealthCheck.Counter
{
    public class CounterCheckSettingsBuilder : ThresholdHealthCheckBuilder<CounterCheckSettings>
    {
        private bool _distributed;

        public CounterCheckSettingsBuilder(string name)
            : base(name)
        {
        }
        
        public CounterCheckSettingsBuilder IsDistributed()
        {
            _distributed = true;
            return this;
        }

        public CounterCheckSettingsBuilder IsLocal()
        {
            _distributed = false;
            return this;
        }

        public override CounterCheckSettings Build()
        {
            return new CounterCheckSettings(Name, Critical, Frequency, Tags, ErrorThreshold, WarningThreshold, _distributed);
        }
    }
}
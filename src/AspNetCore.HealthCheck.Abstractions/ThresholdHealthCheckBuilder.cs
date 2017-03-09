namespace AspNetCore.HealthCheck
{
    public abstract class ThresholdHealthCheckBuilder<TSettings> : SettingsHealthCheckBuilder<TSettings> where TSettings : ThresholdWatchSettings
    {
        private long _errorThreshold;
        private long _warningThreshold;

        public ThresholdHealthCheckBuilder(string name)
            : base(name)
        {
        }

        public long ErrorThreshold => _errorThreshold;

        public long WarningThreshold => _warningThreshold;

        public ThresholdHealthCheckBuilder<TSettings> WithErrorThreshold(long threshold)
        {
            _errorThreshold = threshold;
            return this;
        }

        public ThresholdHealthCheckBuilder<TSettings> WithWarningThreshold(long threshold)
        {
            _warningThreshold = threshold;
            return this;
        }
    }
}
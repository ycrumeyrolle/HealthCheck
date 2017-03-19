using System;

namespace AspNetCore.HealthCheck.Redis
{
    public class RedisHealthCheckBuilder : SettingsHealthCheckBuilder<RedisWatchSettings>
    {
        private string _instance;

        public RedisHealthCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("redis");
        }

        public RedisHealthCheckBuilder WithInstance(string instance)
        {
            _instance = instance;
            return this;
        }
        
        public override RedisWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_instance))
            {
                throw new InvalidOperationException("No Redis instance were defined.");
            }

            return new RedisWatchSettings(Name, Critical, Frequency, Tags, _instance);
        }
    }
}

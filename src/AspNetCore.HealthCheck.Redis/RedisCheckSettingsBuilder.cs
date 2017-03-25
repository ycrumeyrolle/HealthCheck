using System;

namespace AspNetCore.HealthCheck.Redis
{
    public class RedisCheckSettingsBuilder : HealthCheckSettingsBuilder<RedisCheckSettings>
    {
        private string _instance;

        public RedisCheckSettingsBuilder(string name)
            : base(name)
        {
            Tags.Add("redis");
        }

        public RedisCheckSettingsBuilder WithInstance(string instance)
        {
            _instance = instance;
            return this;
        }
        
        public override RedisCheckSettings Build()
        {
            if (string.IsNullOrEmpty(_instance))
            {
                throw new InvalidOperationException("No Redis instance were defined.");
            }

            return new RedisCheckSettings(Name, Critical, Frequency, Tags, _instance);
        }
    }
}

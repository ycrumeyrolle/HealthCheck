using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck.Redis
{
    public static class RedisHealthServiceExtensions
    {
        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, RedisCheckOptions watchOptions)
        {
            var settings = new RedisCheckSettings(watchOptions.Name, watchOptions.Critical, watchOptions.Frequency, watchOptions.Tags, watchOptions.Instance);
            return builder.Add<RedisCheck>(settings);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, Action<RedisCheckSettingsBuilder> configureAction)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var buider = new RedisCheckSettingsBuilder(name);
            configureAction(buider);
            var settings = buider.Build();
            return builder.Add<RedisCheck>(settings);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new RedisCheckOptions();
            configuration.Bind(options);

            return builder.AddRedisCheck(options);
        }
    }
}

using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck.Redis
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, RedisWatchOptions watchOptions)
        {
            var settings = new RedisWatchSettings(watchOptions.Name, watchOptions.Critical, watchOptions.Frequency, watchOptions.Tags, watchOptions.Instance);
            return builder.Add<RedisWatcher>(settings);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, Action<RedisHealthCheckBuilder> configureAction)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var buider = new RedisHealthCheckBuilder(name);
            configureAction(buider);
            var settings = buider.Build();
            return builder.Add<RedisWatcher>(settings);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new RedisWatchOptions();
            configuration.Bind(options);

            return builder.AddRedisCheck(options);
        }
    }
}

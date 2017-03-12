using System;
using Microsoft.Extensions.Configuration;
using AspNetCore.HealthCheck.Oracle;

namespace AspNetCore.HealthCheck
{

    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new OracleWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.AddOracleCheck(settings);
        }

        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, string name, Action<OracleHealthCheckBuilder> configureAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var settingsBuilder = new OracleHealthCheckBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddOracleCheck(settings);
        }

        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new OracleOptions();
            configuration.Bind(options);

            return builder.AddOracleCheck(options);
        }

        private static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleWatchSettings settings)
        {
            return builder.Add<OracleWatcher>(settings);
        }
    }
}

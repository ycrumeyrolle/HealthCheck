using System;
using Microsoft.Extensions.Configuration;
using AspNetCore.HealthCheck.OracleDb;

namespace AspNetCore.HealthCheck
{

    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleDbOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new OracleDbWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.AddOracleCheck(settings);
        }

        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, string name, Action<OracleDbHealthCheckBuilder> configureAction)
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

            var settingsBuilder = new OracleDbHealthCheckBuilder(name);
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

            var options = new OracleDbOptions();
            configuration.Bind(options);

            return builder.AddOracleCheck(options);
        }

        private static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleDbWatchSettings settings)
        {
            return builder.Add<OracleDbWatcher>(settings);
        }
    }
}

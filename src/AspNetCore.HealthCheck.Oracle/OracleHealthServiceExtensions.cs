using System;
using Microsoft.Extensions.Configuration;
using AspNetCore.HealthCheck.Oracle;

namespace AspNetCore.HealthCheck
{

    public static class OracleHealthServiceExtensions
    {
        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleCheckOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new OracleCheckSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.AddOracleCheck(settings);
        }

        public static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, string name, Action<OracleCheckSettingsBuilder> configureAction)
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

            var settingsBuilder = new OracleCheckSettingsBuilder(name);
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

            var options = new OracleCheckOptions();
            configuration.Bind(options);

            return builder.AddOracleCheck(options);
        }

        private static HealthCheckBuilder AddOracleCheck(this HealthCheckBuilder builder, OracleCheckSettings settings)
        {
            return builder.Add<OracleCheck>(settings);
        }
    }
}

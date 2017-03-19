using AspNetCore.HealthCheck.SqlServer;
using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, SqlServerOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new SqlServerSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.AddSqlServerCheck(settings);
        }

        public static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, string name, Action<SqlServerCheckBuilder> configureAction)
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

            var settingsBuilder = new SqlServerCheckBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddSqlServerCheck(settings);
        }

        public static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new SqlServerOptions();
            configuration.Bind(options);

            return builder.AddSqlServerCheck(options);
        }

        private static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, SqlServerSettings settings)
        {
            return builder.Add<SqlServerWatcher>(settings);
        }
    }
}
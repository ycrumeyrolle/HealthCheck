using AspNetCore.HealthCheck.SqlServerDb;
using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, SqlServerDbOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new SqlServerDbSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.AddSqlServerCheck(settings);
        }

        public static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, string name, Action<SqlServerDbCheckBuilder> configureAction)
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

            var settingsBuilder = new SqlServerDbCheckBuilder(name);
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

            var options = new SqlServerDbOptions();
            configuration.Bind(options);

            return builder.AddSqlServerCheck(options);
        }

        private static HealthCheckBuilder AddSqlServerCheck(this HealthCheckBuilder builder, SqlServerDbSettings settings)
        {
            return builder.Add<SqlServerDbWatcher>(settings);
        }
    }
}
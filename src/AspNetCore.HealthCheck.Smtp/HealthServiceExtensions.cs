using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck.Smtp
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, SmtpWatchOptions watchOptions)
        {
            var settings = new SmtpWatchSettings(watchOptions.Name, watchOptions.Critical, watchOptions.Frequency, watchOptions.Tags, watchOptions.SmtpAddress, watchOptions.SmtpPort, watchOptions.UseSsl);
            return builder.Add<SmtpWatcher>(settings);
        }

        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, string name,
            Action<SmtpHealthCheckBuilder> configureAction)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var smtpHealthCheckBuilder = new SmtpHealthCheckBuilder(name);
            configureAction(smtpHealthCheckBuilder);
            var settings = smtpHealthCheckBuilder.Build();
            return builder.Add<SmtpWatcher>(settings);
        }

        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new SmtpWatchOptions();
            configuration.Bind(options);

            return builder.AddSmtpCheck(options);
        }
    }
}

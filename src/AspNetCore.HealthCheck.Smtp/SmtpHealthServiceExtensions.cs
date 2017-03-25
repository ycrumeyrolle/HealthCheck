using System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck.Smtp
{
    public static class SmtpHealthServiceExtensions
    {
        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, SmtpCheckOptions watchOptions)
        {
            var settings = new SmtpCheckSettings(watchOptions.Name, watchOptions.Critical, watchOptions.Frequency, watchOptions.Tags, watchOptions.SmtpAddress, watchOptions.SmtpPort, watchOptions.UseSsl);
            return builder.Add<SmtpCheck>(settings);
        }

        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, string name,
            Action<SmtpCheckSettingsBuilder> configureAction)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var smtpHealthCheckBuilder = new SmtpCheckSettingsBuilder(name);
            configureAction(smtpHealthCheckBuilder);
            var settings = smtpHealthCheckBuilder.Build();
            return builder.Add<SmtpCheck>(settings);
        }

        public static HealthCheckBuilder AddSmtpCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new SmtpCheckOptions();
            configuration.Bind(options);

            return builder.AddSmtpCheck(options);
        }
    }
}

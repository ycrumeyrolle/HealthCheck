using System;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckPolicy
    {
        public HealthCheckPolicy(HealthCheckSettingsCollection checkSettings)
        {
            if (checkSettings == null)
            {
                throw new ArgumentNullException(nameof(checkSettings));
            }

            CheckSettings = checkSettings;
        }

        public HealthCheckSettingsCollection CheckSettings { get; }
    }
}
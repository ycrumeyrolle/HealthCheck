using System;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckPolicy
    {
        public HealthCheckPolicy(SettingsCollection watchSettings)
        {
            if (watchSettings == null)
            {
                throw new ArgumentNullException(nameof(watchSettings));
            }

            WatchSettings = watchSettings;
        }

        public SettingsCollection WatchSettings { get; }
    }
}
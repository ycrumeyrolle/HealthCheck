using System;
using AspNetCore.HealthCheck.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class SystemHealthServiceExtensions
    {
        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, ThresholdCheckOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new FloorThresholdCheckSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ErrorThreshold, options.WarningThreshold);
            return builder.AddVirtualMemorySizeCheck(settings);
        }

        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, string name, Action<VirtualMemorySizeCheckSettingsBuilder> configureAction)
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

            var settingsBuilder = new VirtualMemorySizeCheckSettingsBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddVirtualMemorySizeCheck(settings);
        }

        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new ThresholdCheckOptions();
            configuration.Bind(options);

            return builder.AddVirtualMemorySizeCheck(options);
        }

        private static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, ThresholdCheckSettings settings)
        {
            builder.Services.TryAddTransient<IVirtualMemorySizeProvider, DefaultVirtualMemorySizeProvider>();
            return builder.Add<VirtualMemorySizeCheck>(settings);
        }
        
        public static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, AvailableDiskSpaceCheckOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new AvailableDiskSpaceCheckSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ErrorThreshold, options.WarningThreshold, options.Drive);
            return builder.AddAvailableDiskSpaceCheck(settings);
        }

        public static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, string name, Action<AvailableDiskSpaceCheckSettingsBuilder> configureAction)
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

            var settingsBuilder = new AvailableDiskSpaceCheckSettingsBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddAvailableDiskSpaceCheck(settings);
        }

        public static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new AvailableDiskSpaceCheckOptions();
            configuration.Bind(options);

            return builder.AddAvailableDiskSpaceCheck(options);
        }

        private static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, AvailableDiskSpaceCheckSettings settings)
        {
            builder.Services.TryAddTransient<IAvailableSpaceProvider, DefaultAvailableSpaceProvider>();
            return builder.Add<AvailableDiskSpaceCheck>(settings);
        }
    }
}
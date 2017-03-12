using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AspNetCore.HealthCheck.System;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, ThresholdWatchOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new FloorThresholdWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ErrorThreshold, options.WarningThreshold);
            return builder.AddVirtualMemorySizeCheck(settings);
        }

        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, string name, Action<VirtualMemorySizeHealthCheckBuilder> configureAction)
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

            var settingsBuilder = new VirtualMemorySizeHealthCheckBuilder(name);
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

            var options = new ThresholdWatchOptions();
            configuration.Bind(options);

            return builder.AddVirtualMemorySizeCheck(options);
        }

        private static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, ThresholdWatchSettings settings)
        {
            builder.Services.TryAddTransient<IVirtualMemorySizeProvider, DefaultVirtualMemorySizeProvider>();
            return builder.Add<VirtualMemorySizeWatcher>(settings);
        }
        
        public static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, AvailableDiskSpaceOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new AvailableDiskSpaceWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ErrorThreshold, options.WarningThreshold, options.Drive);
            return builder.AddAvailableDiskSpaceCheck(settings);
        }

        public static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, string name, Action<AvailableDiskSpaceHealthCheckBuilder> configureAction)
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

            var settingsBuilder = new AvailableDiskSpaceHealthCheckBuilder(name);
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

            var options = new AvailableDiskSpaceOptions();
            configuration.Bind(options);

            return builder.AddAvailableDiskSpaceCheck(options);
        }

        private static HealthCheckBuilder AddAvailableDiskSpaceCheck(this HealthCheckBuilder builder, AvailableDiskSpaceWatchSettings settings)
        {
            builder.Services.TryAddTransient<IFreeSpaceProvider, DefaultFreeSpaceProvider>();
            return builder.Add<AvailableDiskSpaceWatcher>(settings);
        }
    }
}
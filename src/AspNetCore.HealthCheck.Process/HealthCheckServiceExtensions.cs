using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, string name, long threshold, string tag, bool critical = false, int frequency = 0)
        {
            return builder.AddVirtualMemorySizeCheck(name, threshold, TagsHelper.FromTag(tag), critical, frequency);
        }

        public static HealthCheckBuilder AddVirtualMemorySizeCheck(this HealthCheckBuilder builder, string name, long threshold, IEnumerable<string> tags = null, bool critical = false, int frequency = 0)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Add<VirtualMemorySizeWatcher>(new VirtualMemorySizeWatchSettings(name, frequency, critical, threshold, tags));
            builder.Services.TryAddTransient(typeof(VirtualMemorySizeWatcher));
            return builder;
        }

        public static HealthCheckBuilder AddAvailableFreeSpaceCheck(this HealthCheckBuilder builder, string name, string drive, long threshold, string tag, bool critical = false, int frequency = 0)
        {
            return builder.AddAvailableFreeSpaceCheck(name, drive, threshold, TagsHelper.FromTag(tag), critical, frequency);
        }

        public static HealthCheckBuilder AddAvailableFreeSpaceCheck(this HealthCheckBuilder builder, string name, string drive, long threshold, IEnumerable<string> tags = null, bool critical = false, int frequency = 0)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Add<AvailableDiskSpaceWatcher>(new AvailableDiskSpaceWatchSettings(name, frequency, critical, drive, threshold, tags));
            builder.Services.TryAddTransient(typeof(AvailableDiskSpaceWatcher));
            return builder;
        }
    }
}
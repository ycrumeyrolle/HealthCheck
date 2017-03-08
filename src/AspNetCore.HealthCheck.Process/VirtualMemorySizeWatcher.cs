using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class VirtualMemorySizeWatcher : HealthWatcher<ThresholdWatchSettings>
    {
        public override Task CheckHealthAsync(HealthContext context, ThresholdWatchSettings settings)
        {
            var virtualMemorySize = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            if (settings.ReachErrorThreshold(virtualMemorySize))
            {
                context.Fail(
                      $"{settings.Name} reach the threshold of {settings.ErrorThreshold} for {virtualMemorySize}",
                    properties: new Dictionary<string, object>
                    {
                        { "virtual_memory_size", virtualMemorySize},
                        { "threshold" , settings.ErrorThreshold }
                    });
            }
            else if (settings.ReachWarningThreshold(virtualMemorySize))
            {
                context.Warn(
                    properties: new Dictionary<string, object>
                    {
                        { "virtual_memory_size", virtualMemorySize},
                        { "threshold" , settings.ErrorThreshold }
                    });
            }
            else
            {
                context.Succeed();
            }

            return TaskCache.CompletedTask;
        }
    }
}
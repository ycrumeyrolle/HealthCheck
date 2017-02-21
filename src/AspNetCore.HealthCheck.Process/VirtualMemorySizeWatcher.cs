using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class VirtualMemorySizeWatcher : HealthWatcher<VirtualMemorySizeWatchSettings>
    {
        public override Task CheckHealthAsync(HealthContext context, VirtualMemorySizeWatchSettings settings)
        {
            var virtualMemorySize = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            if (virtualMemorySize <= settings.Threshold)
            {
                context.Succeed();
            }
            else
            {
                context.Fail(
                    $"{settings.Name} reach the threshold of {settings.Threshold} for {virtualMemorySize}",
                    properties: new Dictionary<string, object>
                    {
                            { "virtual_memory_size", virtualMemorySize},
                            { "threshold" , settings.Threshold }
                    });
            }

            return TaskCache.CompletedTask;
        }
    }
}
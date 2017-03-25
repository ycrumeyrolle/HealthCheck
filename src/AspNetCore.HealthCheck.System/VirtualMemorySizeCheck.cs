using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.System
{
    public class VirtualMemorySizeCheck : HealthCheck<ThresholdCheckSettings>
    {
        private readonly IVirtualMemorySizeProvider _virtualMemorySizeProvider;

        public VirtualMemorySizeCheck(IVirtualMemorySizeProvider virtualMemorySizeProvider)
        {
            _virtualMemorySizeProvider = virtualMemorySizeProvider;
        }

        public override Task CheckHealthAsync(HealthCheckContext context, ThresholdCheckSettings settings)
        {
            var virtualMemorySize = _virtualMemorySizeProvider.GetVirtualMemorySize();
            if (settings.HasReachedErrorThreshold(virtualMemorySize))
            {
                context.Fail(
                      $"{settings.Name} reach the threshold of {settings.ErrorThreshold} for {virtualMemorySize}",
                    properties: new Dictionary<string, object>
                    {
                        { "virtual_memory_size", virtualMemorySize},
                        { "threshold" , settings.ErrorThreshold }
                    });
            }
            else if (settings.HasReachedWarningThreshold(virtualMemorySize))
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
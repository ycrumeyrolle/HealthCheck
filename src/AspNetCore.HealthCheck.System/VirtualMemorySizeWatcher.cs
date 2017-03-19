using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.System
{
    public class VirtualMemorySizeWatcher : HealthWatcher<ThresholdWatchSettings>
    {
        private readonly IVirtualMemorySizeProvider _virtualMemorySizeProvider;

        public VirtualMemorySizeWatcher(IVirtualMemorySizeProvider virtualMemorySizeProvider)
        {
            _virtualMemorySizeProvider = virtualMemorySizeProvider;
        }

        public override Task CheckHealthAsync(HealthContext context, ThresholdWatchSettings settings)
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
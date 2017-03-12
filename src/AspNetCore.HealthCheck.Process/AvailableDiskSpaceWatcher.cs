using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.System
{
    public class AvailableDiskSpaceWatcher : HealthWatcher<AvailableDiskSpaceWatchSettings>
    {
        private readonly IFreeSpaceProvider _freeSpaceProvider;

        public AvailableDiskSpaceWatcher(IFreeSpaceProvider freeSpaceProvider)
        {
            _freeSpaceProvider = freeSpaceProvider;
        }

        public override Task CheckHealthAsync(HealthContext context, AvailableDiskSpaceWatchSettings settings)
        {
            long availableFreeSpace = _freeSpaceProvider.GetAvailableFreeSpace(settings.Drive);
            if (settings.HasReachedErrorThreshold(availableFreeSpace))
            {
                context.Fail(
                    $"Drive {settings.Drive} reach the threshold of {settings.ErrorThreshold} with {availableFreeSpace}",
                    properties: new Dictionary<string, object>
                    {
                            { "drive", settings.Drive },
                            { "warning_threshold" , settings.WarningThreshold },
                            { "error_threshold" , settings.ErrorThreshold },
                            { "available_free_space", availableFreeSpace}
                    });
            }
            else if (settings.HasReachedWarningThreshold(availableFreeSpace))
            {
                context.Warn(
                    properties: new Dictionary<string, object>
                    {
                            { "drive", settings.Drive },
                            { "warning_threshold" , settings.WarningThreshold },
                            { "error_threshold" , settings.ErrorThreshold },
                            { "available_free_space", availableFreeSpace}
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.System
{
    public class AvailableDiskSpaceCheck : HealthCheck<AvailableDiskSpaceCheckSettings>
    {
        private readonly IAvailableSpaceProvider _freeSpaceProvider;

        public AvailableDiskSpaceCheck(IAvailableSpaceProvider freeSpaceProvider)
        {
            _freeSpaceProvider = freeSpaceProvider;
        }

        public override Task CheckHealthAsync(HealthCheckContext context, AvailableDiskSpaceCheckSettings settings)
        {
            long availableFreeSpace = _freeSpaceProvider.GetAvailableDiskSpace(settings.Drive);
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
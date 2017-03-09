using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class AvailableDiskSpaceWatcher : HealthWatcher<AvailableDiskSpaceWatchSettings>
    {
        public override Task CheckHealthAsync(HealthContext context, AvailableDiskSpaceWatchSettings settings)
        {
            var info = new DriveInfo(settings.Drive);

            if (settings.HasReachedErrorThreshold(info.AvailableFreeSpace))
            {
                context.Fail(
                    $"Drive {settings.Drive} reach the threshold of {settings.ErrorThreshold} with {info.AvailableFreeSpace}",
                    properties: new Dictionary<string, object>
                    {
                            { "drive", settings.Drive },
                            { "warning_threshold" , settings.WarningThreshold },
                            { "error_threshold" , settings.ErrorThreshold },
                            { "available_free_space", info.AvailableFreeSpace}
                    });
            }
            else if (settings.HasReachedWarningThreshold(info.AvailableFreeSpace))
            {
                context.Warn(
                    properties: new Dictionary<string, object>
                    {
                            { "drive", settings.Drive },
                            { "warning_threshold" , settings.WarningThreshold },
                            { "error_threshold" , settings.ErrorThreshold },
                            { "available_free_space", info.AvailableFreeSpace}
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
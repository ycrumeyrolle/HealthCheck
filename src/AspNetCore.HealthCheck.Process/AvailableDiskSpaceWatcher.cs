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

            if (info.AvailableFreeSpace > settings.Threshold)
            {
                context.Succeed();
            }
            else
            {
                context.Fail(
                    $"Drive {settings.Drive} reach the threshold of {settings.Threshold} with {info.AvailableFreeSpace}",
                    properties: new Dictionary<string, object>
                    {
                            { "drive", settings.Drive },
                            { "threshold" , settings.Threshold },
                            { "available_free_space", info.AvailableFreeSpace}
                    });
            }

            return TaskCache.CompletedTask;
        }
    }
}
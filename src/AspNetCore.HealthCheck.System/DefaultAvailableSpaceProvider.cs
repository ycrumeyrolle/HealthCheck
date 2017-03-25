using System.IO;

namespace AspNetCore.HealthCheck.System
{
    public class DefaultAvailableSpaceProvider : IAvailableSpaceProvider
    {
        public long GetAvailableDiskSpace (string drive)
        {
            var info = new DriveInfo(drive);
            return info.AvailableFreeSpace;
        }
    }
}
using System.IO;

namespace AspNetCore.HealthCheck.System
{
    public class DefaultFreeSpaceProvider : IFreeSpaceProvider
    {
        public long GetAvailableFreeSpace (string drive)
        {
            var info = new DriveInfo(drive);
            return info.AvailableFreeSpace;
        }
    }
}
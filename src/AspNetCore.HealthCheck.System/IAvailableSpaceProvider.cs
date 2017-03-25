namespace AspNetCore.HealthCheck.System
{
    public interface IAvailableSpaceProvider
    {
        long GetAvailableDiskSpace(string drive);
    }
}
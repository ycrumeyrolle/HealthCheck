namespace AspNetCore.HealthCheck.System
{
    public interface IFreeSpaceProvider
    {
        long GetAvailableFreeSpace(string drive);
    }
}
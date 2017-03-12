namespace AspNetCore.HealthCheck.System
{
    public interface IVirtualMemorySizeProvider
    {
        long GetVirtualMemorySize();
    }
}
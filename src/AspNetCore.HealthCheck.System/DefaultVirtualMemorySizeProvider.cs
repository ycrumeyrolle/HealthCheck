using System.Diagnostics;

namespace AspNetCore.HealthCheck.System
{
    public class DefaultVirtualMemorySizeProvider : IVirtualMemorySizeProvider
    {
        private readonly Process _process;

        public DefaultVirtualMemorySizeProvider()
        {
            _process = Process.GetCurrentProcess();
        }
        public long GetVirtualMemorySize()
        {
            return _process.VirtualMemorySize64;
        }
    }
}
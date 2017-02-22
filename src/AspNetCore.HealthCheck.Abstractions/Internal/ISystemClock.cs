using System;

namespace AspNetCore.HealthCheck
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
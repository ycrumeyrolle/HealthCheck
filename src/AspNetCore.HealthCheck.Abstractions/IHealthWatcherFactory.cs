using System;

namespace AspNetCore.HealthCheck
{
    public interface IHealthWatcherFactory
    {
        IHealthWatcher Create(Type watcherType);
    }
}
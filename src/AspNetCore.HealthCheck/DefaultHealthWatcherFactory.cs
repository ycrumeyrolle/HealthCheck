using System;

namespace AspNetCore.HealthCheck
{
    public class DefaultHealthWatcherFactory : IHealthWatcherFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultHealthWatcherFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHealthWatcher Create(Type watcherType)
        {
            return (IHealthWatcher)_serviceProvider.GetService(watcherType);
        }
    }
}
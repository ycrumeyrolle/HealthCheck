using System;

namespace AspNetCore.HealthCheck
{
    public class DefaultCheckFactory : ICheckFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultCheckFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHealthCheck Create(Type checkType)
        {
            return (IHealthCheck)_serviceProvider.GetService(checkType);
        }
    }
}
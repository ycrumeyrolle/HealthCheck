using System;

namespace AspNetCore.HealthCheck
{
    public interface ICheckFactory
    {
        IHealthCheck Create(Type checkType);
    }
}
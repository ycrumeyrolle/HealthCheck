using System;
using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckSettingsCollection : List<KeyValuePair<Type, IHealthCheckSettings>>
    {
        public void Add(Type type, IHealthCheckSettings settings)
        {
            Add(new KeyValuePair<Type, IHealthCheckSettings>(type, settings));
        }
    }
}
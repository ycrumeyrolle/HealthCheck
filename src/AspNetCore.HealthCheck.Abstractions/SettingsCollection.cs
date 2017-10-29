using System;
using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class SettingsCollection : List<KeyValuePair<Type, IWatchSettings>>
    {
        public void Add(Type type, IWatchSettings settings)
        {
            Add(new KeyValuePair<Type, IWatchSettings>(type, settings));
        }
    }
}
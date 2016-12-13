using System;
using System.Collections.Generic;

namespace AspNetCore.Counter
{
    public class LocalCounterProvider : ICounterProvider
    {
        private readonly Dictionary<string, ICounter> _counters;

        public LocalCounterProvider()
        {
            _counters = new Dictionary<string, ICounter>(StringComparer.Ordinal);
        }

        public ICounter GetCounter(string name, bool distributed)
        {
            ICounter counter;
            if (!_counters.TryGetValue(name, out counter))
            {
                counter = new LocalCounter(name);
                _counters[name] = counter;
            }

            return counter;
        }
    }
}
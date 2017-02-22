using System.Collections.Generic;

namespace AspNetCore.Counter
{
    public abstract class CompositeCounterProvider : ICounterProvider
    {
        private readonly LocalCounterProvider _localCounterProvider;
        private readonly Dictionary<string, ICounter> _counters;

        public CompositeCounterProvider(LocalCounterProvider localCounterProvider)
        {
            _localCounterProvider = localCounterProvider;
        }

        protected abstract ICounter CreateCounter(string name);
                
        public virtual ICounter GetCounter(string name, bool distributed)
        {
            if (!distributed)
            {
                return _localCounterProvider.GetCounter(name, false);
            }

            ICounter counter;
            if (!_counters.TryGetValue(name, out counter))
            {
                counter = CreateCounter(name);
                _counters[name] = counter;
            }

            return counter;
        }
    }
}
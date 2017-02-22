using System;
using System.Threading;

namespace AspNetCore.Counter
{
    public class LocalCounter : ICounter
    {
        private long _value = 0L;

        public LocalCounter(string name)
        {
            Name = name;
        }
        
        public string Name { get; }

        public long Value
        {
            get
            {
                return _value;
            }

            set
            {
                Interlocked.Exchange(ref _value, value);
            }
        }

        public long Decrement(long decrementBy)
        {
            Interlocked.Add(ref _value, -decrementBy);
            return _value;
        }

        public long Increment(long incrementBy)
        {
            Interlocked.Add(ref _value, incrementBy);
            return _value;
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _value, 0L);
        }
    }
}

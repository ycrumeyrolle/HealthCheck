using StackExchange.Redis;

namespace AspNetCore.Counter.Redis
{
    public class RedisCounter : ICounter
    {
        private readonly IDatabase _database;

        public RedisCounter(string name, IDatabase database)
        {
            Name = name;
            _database = database;
        }

        public string Name { get; }

        public long Value
        {
            get
            {
                return (long)_database.StringGet(Name);
            }

            set
            {
                _database.StringSet(Name, value);
            }
        }

        public long Decrement(long decrementBy = 1L)
        {
            return _database.StringDecrement(Name, decrementBy);
        }

        public long Increment(long incrementBy = 1L)
        {
            return _database.StringIncrement(Name, incrementBy);
        }

        public void Reset()
        {
            _database.StringSet(Name, 0L);
        }
    }
}

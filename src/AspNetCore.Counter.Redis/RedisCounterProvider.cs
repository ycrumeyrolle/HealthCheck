using StackExchange.Redis;

namespace AspNetCore.Counter.Redis
{
    public class RedisCounterProvider : CompositeCounterProvider
    {
        private readonly IDatabase _database;

        public RedisCounterProvider(IDatabase database, LocalCounterProvider localCounterProvider) 
            : base(localCounterProvider)
        {
            _database = database;
        }

        protected override ICounter CreateCounter(string name)
        {
            return new RedisCounter(name, _database);
        }
    }
}
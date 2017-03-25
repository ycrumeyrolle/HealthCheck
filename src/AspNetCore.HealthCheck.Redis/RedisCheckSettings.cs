using System.Collections.Generic;
using System.Threading;
using StackExchange.Redis;

namespace AspNetCore.HealthCheck.Redis
{
    public class RedisCheckSettings : HealthCheckSettings
    {
        private volatile ConnectionMultiplexer _connection;
        private IDatabase _database;

        private readonly string _instance;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public RedisCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string instance)
            : base(name, critical, frequency, tags)
        {
            _instance = instance;
        }

        public IDatabase Database
        {
            get
            {
                Connect();
                return _database;
            }

            set
            {
                _database = value;
            }
        }

        private void Connect()
        {
            if (_database != null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_connection == null)
                {
                    _connection = ConnectionMultiplexer.Connect(_instance);
                    _database = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
    }
}
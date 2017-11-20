using System.Collections.Generic;
using MongoDB.Driver;

namespace AspNetCore.HealthCheck.Mongo
{
    public class MongoSettings : HealthCheckSettings
    {
        public MongoSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string connectionString) 
            : base(name, critical, frequency, tags)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            this.Database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase Database { get; private set; }    
    }
}
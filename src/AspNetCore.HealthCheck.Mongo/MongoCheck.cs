using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AspNetCore.HealthCheck.Mongo
{
    public class MongoCheck : HealthCheck<MongoSettings>
    {
        public override async Task CheckHealthAsync(HealthCheckContext context, MongoSettings settings)
        {
            IAsyncCursor<BsonDocument> asyncCursor = await settings.Database.ListCollectionsAsync(null, context.CancellationToken);
            await asyncCursor.ToListAsync();
            context.Succeed();
        }
    }
}


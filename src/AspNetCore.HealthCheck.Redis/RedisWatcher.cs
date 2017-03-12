using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Redis
{
    public class RedisWatcher : HealthWatcher<RedisWatchSettings>
    {
        public override async Task CheckHealthAsync(HealthContext context, RedisWatchSettings settings)
        {             
            await settings.Database.PingAsync();
            context.Succeed();
        }
    }
}

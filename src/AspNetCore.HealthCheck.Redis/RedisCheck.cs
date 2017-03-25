using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Redis
{
    public class RedisCheck : HealthCheck<RedisCheckSettings>
    {
        public override async Task CheckHealthAsync(HealthCheckContext context, RedisCheckSettings settings)
        {             
            await settings.Database.PingAsync();
            context.Succeed();
        }
    }
}

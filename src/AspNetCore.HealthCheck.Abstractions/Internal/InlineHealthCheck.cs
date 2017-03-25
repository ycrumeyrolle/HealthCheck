using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class InlineHealthCheck : HealthCheck<InlineHealthCheckSettings>
    {
        public override Task CheckHealthAsync(HealthCheckContext context, InlineHealthCheckSettings settings)
        {
            return settings.Action(context);
        }
    }
}
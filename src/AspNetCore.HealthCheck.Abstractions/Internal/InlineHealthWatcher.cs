using System.Threading.Tasks;
using AspNetCore.HealthCheck;

namespace AspNetCore.HealthCheck
{
    public class InlineHealthWatcher : HealthWatcher<InlineSettings>
    {
        public override Task CheckHealthAsync(HealthContext context, InlineSettings settings)
        {
            return settings.Action(context);
        }
    }
}
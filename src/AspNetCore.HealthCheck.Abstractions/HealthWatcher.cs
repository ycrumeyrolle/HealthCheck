using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public abstract class HealthWatcher<TWatchSettings> : IHealthWatcher where TWatchSettings : IWatchSettings
    {
        public async Task CheckHealthAsync(HealthContext context)
        {
            context.Stopwatch.Start();
            try
            {
                await CheckHealthAsync(context, (TWatchSettings)context.Settings);
            }
            finally
            {
                context.Stopwatch.Start();
            }
        }

        public abstract Task CheckHealthAsync(HealthContext context, TWatchSettings settings);
    }
}
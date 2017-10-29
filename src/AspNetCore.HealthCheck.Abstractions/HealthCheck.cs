using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public abstract class HealthCheck<THealthCheckSettings> : IHealthCheck where THealthCheckSettings : IHealthCheckSettings
    {
        public async Task CheckHealthAsync(HealthCheckContext context)
        {
            context.Stopwatch.Start();
            try
            {
                await CheckHealthAsync(context, (THealthCheckSettings)context.Settings);
            }
            finally
            {
                context.Stopwatch.Stop();
            }
        }

        public abstract Task CheckHealthAsync(HealthCheckContext context, THealthCheckSettings settings);
    }
}
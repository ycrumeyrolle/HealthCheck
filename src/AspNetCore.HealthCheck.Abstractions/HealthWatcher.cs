using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public abstract class HealthWatcher<TWatchSettings> : IHealthWatcher where TWatchSettings : IWatchSettings
    {
        public async Task CheckHealthAsync(HealthContext context)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                await CheckHealthAsync(context, (TWatchSettings)context.Settings);
            }
            catch (Exception e)
            {
                HandleException(context, e);
            }
            finally
            {
                context.Elapsed = stopwatch.ElapsedMilliseconds;
            }
        }

        public abstract Task CheckHealthAsync(HealthContext context, TWatchSettings settings);

        public virtual void HandleException(HealthContext context, Exception exception)
        {
            context.Fail(exception.Message);
        }
    }
}
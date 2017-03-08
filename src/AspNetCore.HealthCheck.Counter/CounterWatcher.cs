using AspNetCore.Counter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Counter
{
    public class CounterWatcher : HealthWatcher<CounterWatchSettings>
    {
        private readonly ICounterProvider _counterProvider;

        public CounterWatcher(ICounterProvider counterProvider)
        {
            _counterProvider = counterProvider;
        }
        
        public override Task CheckHealthAsync(HealthContext context, CounterWatchSettings settings)
        {
            var counter = _counterProvider.GetCounter(settings.Name, settings.Distributed);

            if (settings.ReachErrorThreshold(counter.Value))
            {
                context.Fail(
                    $"Counter {settings.Name} reach the threshold of {settings.ErrorThreshold} for {counter.Value}",
                    properties: new Dictionary<string, object>
                    {
                            { "counter", counter.Value },
                            { "threshold" , settings.ErrorThreshold }
                    });
            }
            else if (settings.ReachWarningThreshold(counter.Value))
            {
                context.Warn(
                    properties: new Dictionary<string, object>
                    {
                        { "counter", counter.Value },
                        { "threshold" , settings.ErrorThreshold }
                    });
            }
            else
            {
                context.Succeed(
                    properties: new Dictionary<string, object>
                    {
                        { "counter", counter.Value },
                        { "threshold" , settings.ErrorThreshold }
                    });
            }

            return TaskCache.CompletedTask;
        }
    }
}
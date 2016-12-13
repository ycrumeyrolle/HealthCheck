using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Counter;

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

            if (counter.Value <= settings.Threshold)
            {
                context.Succeed(
                    properties: new Dictionary<string, object>
                    {
                        { "counter", counter.Value },
                        { "threshold" , settings.Threshold }
                    });
            }
            else
            {
                context.Fail(
                    $"Counter {settings.Name} reach the threshold of {settings.Threshold} for {counter.Value}",
                    properties: new Dictionary<string, object>
                    {
                        { "counter", counter.Value },
                        { "threshold" , settings.Threshold }
                    });
            }

            return TaskCache.CompletedTask;
        }
    }
}
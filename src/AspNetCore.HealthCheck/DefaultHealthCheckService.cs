using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AspNetCore.HealthCheck
{
    public class DefaultHealthCheckService : IHealthCheckService
    {
        private readonly ISystemClock _clock;
        private readonly IHealthWatcherFactory _watcherFactory;
        private readonly Dictionary<string, WatchResultCache> _resultCache;
        private readonly HealthCheckPolicy _defaultPolicy;

        public DefaultHealthCheckService(ISystemClock clock, IHealthCheckPolicyProvider policyProvider, IHealthWatcherFactory watcherFactory)
        {
            if (watcherFactory == null)
            {
                throw new ArgumentNullException(nameof(watcherFactory));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            _watcherFactory = watcherFactory;
            _clock = clock;
            _defaultPolicy = defaultPolicy;
            _resultCache = CreateCache();
        }

        private Dictionary<string, WatchResultCache> CreateCache()
        {
            var results = new Dictionary<string, WatchResultCache>();

            foreach (var kvpSettings in _defaultPolicy.WatchSettings)
            {
                var watcherType = kvpSettings.Key;
                var settings = kvpSettings.Value;
                IHealthWatcher watcher = _watcherFactory.Create(watcherType);
                results.Add(settings.Name, new WatchResultCache(watcher));
            }

            return results;
        }

        public async Task<HealthResponse> CheckHealthAsync(HealthCheckPolicy policy)
        {
            var taskResults = new Task<HealthCheckResult>[policy.WatchSettings.Count];
            for (int i = 0; i < policy.WatchSettings.Count; i++)
            {
                var settings = policy.WatchSettings[i].Value;
                taskResults[i] = Task.Run(() => CheckHealthAsync(policy, settings));
            }

            var results = await Task.WhenAll(taskResults);
            return new HealthResponse(results);
        }

        private async Task<HealthCheckResult> CheckHealthAsync(HealthCheckPolicy policy, IWatchSettings settings)
        {
            var watchCache = _resultCache[settings.Name];

            var utcNow = _clock.UtcNow;
            HealthCheckResult result = null;
            if (!watchCache.ShouldCheck(utcNow))
            {
                result = watchCache.Result;
            }
            else
            {
                using (CancellationTokenSource cts = new CancellationTokenSource(settings.Timeout))
                {
                    IHealthWatcher watcher = watchCache.Watcher;
                    var healthContext = new HealthContext(settings);
                    healthContext.CancellationToken = cts.Token;
                    try
                    {
                        await watcher.CheckHealthAsync(healthContext);
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Elapsed = healthContext.ElapsedMilliseconds,
                            Message = healthContext.Message,
                            Status = healthContext.HasSucceeded ? HealthStatus.OK : healthContext.HasWarned ? HealthStatus.Warning : HealthStatus.KO,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            NextTry = utcNow.AddSeconds(settings.Frequency),
                            Critical = settings.Critical,
                            Properties = healthContext.Properties?.ToDictionary(kvp => kvp.Key, kvp => JToken.FromObject(kvp.Value))
                        };
                        
                        watchCache.Result = result;
                    }
                    catch (Exception e)
                    {
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Elapsed = healthContext.ElapsedMilliseconds,
                            Message = "An error occured. See logs for more details.",
                            Status = HealthStatus.KO,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            NextTry = utcNow.AddSeconds(settings.Frequency),
                            Critical = settings.Critical,
                            Exception = e
                        };
                        watchCache.Result = result;
                    }
                }
            }

            return result;
        }

        private class WatchResultCache
        {
            public WatchResultCache(IHealthWatcher watcher)
            {
                Watcher = watcher;
            }

            public IHealthWatcher Watcher { get; }

            public HealthCheckResult Result { get; set; }

            public bool ShouldCheck(DateTimeOffset time)
            {
                return Result == null || Result.NextTry < time;
            }
        }
    }
}
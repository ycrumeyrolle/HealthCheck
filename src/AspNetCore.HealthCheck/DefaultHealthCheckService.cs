using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AspNetCore.HealthCheck
{
    public class DefaultHealthCheckService : IHealthCheckService
    {
        private readonly ISystemClock _clock;
        private readonly ICheckFactory _checkFactory;
        private readonly Dictionary<string, CheckResultCache> _resultCache;
        private readonly HealthCheckPolicy _defaultPolicy;

        public DefaultHealthCheckService(ISystemClock clock, IHealthCheckPolicyProvider policyProvider, ICheckFactory checkFactory)
        {
            if (checkFactory == null)
            {
                throw new ArgumentNullException(nameof(checkFactory));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            _checkFactory = checkFactory;
            _clock = clock;
            _defaultPolicy = policyProvider.DefaultPolicy;
            _resultCache = CreateCache();
        }

        private Dictionary<string, CheckResultCache> CreateCache()
        {
            var results = new Dictionary<string, CheckResultCache>();

            foreach (var kvpSettings in _defaultPolicy.CheckSettings)
            {
                var checkType = kvpSettings.Key;
                var settings = kvpSettings.Value;
                IHealthCheck check = _checkFactory.Create(checkType);
                results.Add(settings.Name, new CheckResultCache(check));
            }

            return results;
        }

        public async Task<HealthCheckResponse> CheckHealthAsync(HealthCheckPolicy policy)
        {
            var taskResults = new Task<HealthCheckResult>[policy.CheckSettings.Count];
            for (int i = 0; i < policy.CheckSettings.Count; i++)
            {
                var settings = policy.CheckSettings[i].Value;
                taskResults[i] = Task.Run(() => CheckHealthAsync(policy, settings));
            }

            var results = await Task.WhenAll(taskResults);
            return new HealthCheckResponse(results);
        }

        private async Task<HealthCheckResult> CheckHealthAsync(HealthCheckPolicy policy, IHealthCheckSettings settings)
        {
            var checkCache = _resultCache[settings.Name];

            var utcNow = _clock.UtcNow;
            HealthCheckResult result = null;
            if (!checkCache.ShouldCheck(utcNow))
            {
                result = checkCache.Result;
            }
            else
            {
                using (CancellationTokenSource cts = new CancellationTokenSource(settings.Timeout))
                {
                    IHealthCheck check = checkCache.Check;
                    var healthContext = new HealthCheckContext(settings);
                    healthContext.CancellationToken = cts.Token;
                    try
                    {
                        await check.CheckHealthAsync(healthContext);
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Elapsed = healthContext.ElapsedMilliseconds,
                            Message = healthContext.Message,
                            Status = healthContext.HasSucceeded ? HealthStatus.Healthy : healthContext.HasWarned ? HealthStatus.Warning : HealthStatus.Unhealthy,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            NextTry = utcNow.AddSeconds(settings.Frequency),
                            Critical = settings.Critical,
                            Properties = healthContext.Properties?.ToDictionary(kvp => kvp.Key, kvp => JToken.FromObject(kvp.Value))
                        };
                        
                        checkCache.Result = result;
                    }
                    catch (Exception e)
                    {
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Elapsed = healthContext.ElapsedMilliseconds,
                            Message = "An error occured. See logs for more details.",
                            Status = HealthStatus.Unhealthy,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            NextTry = utcNow.AddSeconds(settings.Frequency),
                            Critical = settings.Critical,
                            Exception = e
                        };
                        checkCache.Result = result;
                    }
                }
            }

            return result;
        }

        private class CheckResultCache
        {
            public CheckResultCache(IHealthCheck check)
            {
                Check = check;
            }

            public IHealthCheck Check { get; }

            public HealthCheckResult Result { get; set; }

            public bool ShouldCheck(DateTimeOffset time)
            {
                return Result == null || Result.NextTry < time;
            }
        }
    }
}
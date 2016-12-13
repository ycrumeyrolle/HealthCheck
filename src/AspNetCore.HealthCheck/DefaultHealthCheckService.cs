﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AspNetCore.HealthCheck
{
    public class DefaultHealthCheckService : IHealthCheckService
    {
        private readonly ISystemClock _clock;
        private HealthResponse _latestResult = new HealthResponse();
        private readonly IHealthWatcherFactory _watcherFactory;
        private readonly ILogger _logger;
        private readonly Dictionary<string, WatchResultCache> _resultCache;
        private readonly HealthCheckPolicy _defaultPolicy;

        public DefaultHealthCheckService(ISystemClock clock, HealthCheckPolicy defaultPolicy, IHealthWatcherFactory watcherFactory, ILoggerFactory loggerFactory)
        {
            if (watcherFactory == null)
            {
                throw new ArgumentNullException(nameof(watcherFactory));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (defaultPolicy == null)
            {
                throw new ArgumentNullException(nameof(defaultPolicy));
            }

            _watcherFactory = watcherFactory;
            _clock = clock;
            _defaultPolicy = defaultPolicy;
            _resultCache = CreateCache();
            _logger = loggerFactory.CreateLogger<DefaultHealthCheckService>();
        }

        private Dictionary<string, WatchResultCache> CreateCache()
        {
            var results = new Dictionary<string, WatchResultCache>();

            foreach (var kvpSettings in _defaultPolicy.WatchSettings)
            {
                var watcherType = kvpSettings.Key;
                var settings = kvpSettings.Value;
                IHealthWatcher watcher = null;
                watcher = _watcherFactory.Create(watcherType);
                results.Add(settings.Name, new WatchResultCache(watcher, settings));
            }

            return results;
        }

        public async Task<HealthResponse> CheckHealthAsync(HealthCheckPolicy policy)
        {
            var healthResults = new List<HealthCheckResult>();
            var stopwatch = new Stopwatch();

            for (int i = 0; i < policy.WatchSettings.Count; i++)
            {
                var settings = policy.WatchSettings[i].Value;
                var watchCache = _resultCache[settings.Name];

                var utcNow = _clock.UtcNow;
                HealthCheckResult result = null;
                if (!watchCache.ShouldCheck(utcNow))
                {
                    result = watchCache.Result;
                    healthResults.Add(result);
                }
                else
                {
                    IHealthWatcher watcher = watchCache.Watcher;
                    var healthContext = new HealthContext(settings);
                    try
                    {
                        await watcher.CheckHealthAsync(healthContext);
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Tags = settings.Tags,
                            Elapsed = healthContext.Elapsed,
                            Message = healthContext.Message,
                            Status = healthContext.HasSucceeded ? HealthStatus.OK : healthContext.HasWarned ? HealthStatus.Warning : HealthStatus.KO,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            Critical = settings.Critical,
                            Properties = healthContext.Properties?.ToDictionary(kvp => kvp.Key, kvp => JToken.FromObject(kvp.Value))
                        };

                        watchCache.Update(_clock.UtcNow, result);
                    }
                    catch (Exception e)
                    {
                        result = new HealthCheckResult
                        {
                            Name = settings.Name,
                            Tags = settings.Tags,
                            Elapsed = stopwatch.ElapsedMilliseconds,
                            // TODO : Should we provide the exception message ?
                            Message = e.Message,
                            Status = HealthStatus.KO,
                            Issued = utcNow.ToUnixTimeSeconds(),
                            Critical = settings.Critical,
                        };
                        _logger.LogError(0, e, e.Message);
                    }
                    finally
                    {
                        healthResults.Add(result);
                    }
                }
            }

            return new HealthResponse(healthResults);
        }

        private class WatchResultCache
        {
            public WatchResultCache(IHealthWatcher watcher, IWatchSettings settings)
            {
                Watcher = watcher;
                Settings = settings;
            }

            public IHealthWatcher Watcher { get; }

            public IWatchSettings Settings { get; }

            public HealthCheckResult Result { get; private set; }

            public DateTimeOffset Latest { get; private set; }

            public void Update(DateTimeOffset time, HealthCheckResult result)
            {
                Latest = time;
                Result = result;
            }

            public bool ShouldCheck(DateTimeOffset time)
            {
                return Latest.AddSeconds(Settings.Frequency) < time || Result == null;
            }
        }
    }
}
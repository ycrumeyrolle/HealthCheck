using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckMiddleware
    {
        private const string ApplicationJson = "application/json";

        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly HealthCheckOptions _options;
        private readonly JsonSerializer serializer = new JsonSerializer();
        private readonly ArrayPool<char> _charPool;
        private readonly ArrayPool<byte> _bytePool;
        private readonly JsonArrayPool<char> _jsonCharPool;
        private readonly IHealthCheckService _healthService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly HealthCheckPolicy _defaultPolicy;
        private readonly Dictionary<string, HealthCheckPolicy> _subPolicies;
        private readonly IServerSwitch _serverSwitch;

        public HealthCheckMiddleware(
            RequestDelegate next, 
            IOptions<HealthCheckOptions> options, 
            ILoggerFactory loggerFactory, 
            IHealthCheckService healthService, 
            HealthCheckPolicy defaultPolicy, 
            IServerSwitch serverSwitch)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (healthService == null)
            {
                throw new ArgumentNullException(nameof(healthService));
            }

            if (defaultPolicy == null)
            {
                throw new ArgumentNullException(nameof(defaultPolicy));
            }
            
            _next = next;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<HealthCheckMiddleware>();
            _healthService = healthService;
            _defaultPolicy = defaultPolicy;
            _serverSwitch = serverSwitch;
            _subPolicies = CreateSubPolicies(_defaultPolicy);

            _charPool = ArrayPool<char>.Shared;
            _jsonCharPool = new JsonArrayPool<char>(_charPool);
            _bytePool = ArrayPool<byte>.Shared;
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter
                    {
                        CamelCaseText = true
                    }
                },
                NullValueHandling = NullValueHandling.Ignore
            };
            _jsonSerializer = JsonSerializer.Create(jsonSettings);
        }

        private Dictionary<string, HealthCheckPolicy> CreateSubPolicies(HealthCheckPolicy policy)
        {
            var result = policy.WatchSettings
                .SelectMany(s => s.Value.Tags, (s, t) => new { Tag = t, Settings = s })
                .GroupBy(item => item.Tag)
                .ToDictionary(
                    group => group.Key,
                    group => group.Aggregate(new SettingsCollection(), (settings, item) =>
                    {
                        settings.Add(item.Settings);
                        return settings;
                    },
                    settings => new HealthCheckPolicy(settings)));
            return result;
        }

        public async Task Invoke(HttpContext context)
        {
            PathString remaining;
            if (!context.Request.Path.StartsWithSegments(_options.Path, out remaining))
            {
                await _next(context);
                return;
            }

            var serverSwitchContext = new ServerSwitchContext(context);
            await _serverSwitch?.CheckServerState(serverSwitchContext);
            var response = context.Response;
            if (serverSwitchContext.ServerDisabled)
            {
                _logger.ServerDisabled();
                response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                response.Headers[HeaderNames.CacheControl] = "no-cache";
                response.Headers[HeaderNames.Pragma] = "no-cache";
                response.Headers[HeaderNames.Expires] = "-1";
                if (serverSwitchContext.RetryAfter.HasValue)
                {
                    response.Headers[HeaderNames.RetryAfter] = serverSwitchContext.RetryAfter.Value.ToString(NumberFormatInfo.InvariantInfo);
                }
            }

            if (!serverSwitchContext.ServerDisabled || _options.CheckHealthEvenDisabled)
            {
                HealthCheckPolicy policy;
                if (!_subPolicies.TryGetValue(remaining.ToUriComponent().TrimStart('/'), out policy))
                {
                    policy = _defaultPolicy;
                }

                var healthCheckResponse = await _healthService.CheckHealthAsync(policy);
                if (healthCheckResponse.HasErrors)
                {
                    response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                }
                else
                {
                    _logger.HealthCheckSucceeded();
                    if (!serverSwitchContext.ServerDisabled)
                    {
                        response.StatusCode = StatusCodes.Status200OK;
                    }
                }

                if (_options.SendResults)
                {
                    response.ContentType = ApplicationJson;
                    using (var writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8, 1024, _bytePool, _charPool))
                    {
                        using (var jsonWriter = new JsonTextWriter(writer))
                        {
                            jsonWriter.ArrayPool = _jsonCharPool;
                            jsonWriter.CloseOutput = false;

                            _jsonSerializer.Serialize(jsonWriter, healthCheckResponse.Results);
                        }
                    }
                }
            }
        }
    }
}
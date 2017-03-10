using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckMiddleware
    {
        private const string ApplicationJson = "application/json";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly HealthCheckOptions _options;
        private readonly IHealthCheckService _healthService;
        private readonly IHealthCheckPolicyProvider _policyProvider;
        private readonly IAuthorizationService _authorizationService;

        private readonly JsonSerializer _jsonSerializer;
        private readonly JsonSerializer serializer = new JsonSerializer();
        private readonly ArrayPool<char> _charPool;
        private readonly ArrayPool<byte> _bytePool;
        private readonly JsonArrayPool<char> _jsonCharPool;

        public HealthCheckMiddleware(
            RequestDelegate next,
            IOptions<HealthCheckOptions> options,
            ILoggerFactory loggerFactory,
            IHealthCheckService healthService,
            IHealthCheckPolicyProvider policyProvider,
            IAuthorizationService authorizationService)
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

            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            if (authorizationService == null)
            {
                throw new ArgumentNullException(nameof(authorizationService));
            }

            _next = next;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<HealthCheckMiddleware>();
            _healthService = healthService;
            _policyProvider = policyProvider;
            _authorizationService = authorizationService;

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
        
        public async Task Invoke(HttpContext context)
        {
            PathString subpath;
            if (!context.Request.Path.StartsWithSegments(_options.Path, out subpath))
            {
                await _next(context);
                return;
            }

            var policyName = subpath.ToUriComponent().Trim('/');
            if (policyName.Length == 0)
            {
                policyName = Constants.DefaultPolicy;
            }
            
            HealthCheckPolicy policy = _policyProvider.GetPolicy(policyName);
            if (policy == null)
            {
                await _next(context);
                return;
            }

            if (_options.AuthorizationPolicy != null)
            {
                // Build a ClaimsPrincipal with the Policy's required authentication types
                if (_options.AuthorizationPolicy.AuthenticationSchemes != null && _options.AuthorizationPolicy.AuthenticationSchemes.Any())
                {
                    ClaimsPrincipal newPrincipal = null;
                    foreach (var scheme in _options.AuthorizationPolicy.AuthenticationSchemes)
                    {
                        var result = await context.Authentication.AuthenticateAsync(scheme);
                        if (result != null)
                        {
                            newPrincipal = SecurityHelper.MergeUserPrincipal(newPrincipal, result);
                        }
                    }

                    // If all schemes failed authentication, provide a default identity anyways
                    if (newPrincipal == null)
                    {
                        newPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                    }

                    context.User = newPrincipal;
                }

                // Note: Default Anonymous User is new ClaimsPrincipal(new ClaimsIdentity())
                if (!await _authorizationService.AuthorizeAsync(context.User, context, _options.AuthorizationPolicy))
                {
                    _logger.AuthorizationFailed();
                    await _next(context);
                    return;
                }
            }

            var response = context.Response;
            var healthCheckResponse = await _healthService.CheckHealthAsync(policy);
            if (healthCheckResponse.HasCriticalErrors)
            {
                response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
            else
            {
                response.StatusCode = StatusCodes.Status200OK;
                _logger.HealthCheckSucceeded();
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
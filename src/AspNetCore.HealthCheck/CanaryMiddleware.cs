using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AspNetCore.HealthCheck
{
    public class CanaryMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly CanaryOptions _options;
        private readonly IHealthCheckService _healthService;
        private readonly HealthCheckPolicy _defaultPolicy;
        private readonly IServerSwitch _serverSwitch;
        private readonly IAuthorizationService _authorizationService;

        public CanaryMiddleware(
            RequestDelegate next,
            IOptions<CanaryOptions> options,
            ILoggerFactory loggerFactory,
            IHealthCheckService healthService,
            IHealthCheckPolicyProvider policyProvider,
            IServerSwitch serverSwitch,
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
            _logger = loggerFactory.CreateLogger<CanaryMiddleware>();
            _healthService = healthService;
            _serverSwitch = serverSwitch;
            _authorizationService = authorizationService;

            if (_options.EnableHealthCheck)
            {
                var defaultPolicy = policyProvider.GetPolicy(_options.PolicyName);
                if (defaultPolicy == null)
                {
                    throw new ArgumentException($"{nameof(_options.PolicyName)} '{_options.PolicyName}' is not a valid policy.", nameof(_options));
                }

                _defaultPolicy = defaultPolicy;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            if (_options.AuthorizationPolicy != null)
            {
                var principal = await SecurityHelper.GetUserPrincipal(context, _options.AuthorizationPolicy);
                
                if (!await _authorizationService.AuthorizeAsync(principal, context, _options.AuthorizationPolicy))
                {
                    _logger.AuthorizationFailed();
                    await _next(context);
                    return;
                }
            }

            var response = context.Response;

            // Canary response must not be cached
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            var serverSwitchContext = new ServerSwitchContext(context);
            await _serverSwitch?.CheckServerStateAsync(serverSwitchContext);
            if (serverSwitchContext.ServerDisabled)
            {
                _logger.ServerDisabled();
                response.StatusCode = StatusCodes.Status503ServiceUnavailable;                
                if (serverSwitchContext.RetryAfter.HasValue)
                {
                    response.Headers[HeaderNames.RetryAfter] = serverSwitchContext.RetryAfter.Value.ToString(NumberFormatInfo.InvariantInfo);
                }
            }
            else if(_options.EnableHealthCheck)
            {
                var healthCheckResponse = await _healthService.CheckHealthAsync(_defaultPolicy);
                if (healthCheckResponse.HasCriticalErrors)
                {
                    response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status200OK;
                }
            }
        }
    }
}
using System;
using System.Globalization;
using System.Threading.Tasks;
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

        public CanaryMiddleware(
            RequestDelegate next,
            IOptions<CanaryOptions> options,
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
            _logger = loggerFactory.CreateLogger<CanaryMiddleware>();
            _healthService = healthService;
            _defaultPolicy = defaultPolicy;
            _serverSwitch = serverSwitch;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            var serverSwitchContext = new ServerSwitchContext(context);
            await _serverSwitch?.CheckServerStateAsync(serverSwitchContext);
            var response = context.Response;

            // Canary response must not be cached
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            if (serverSwitchContext.ServerDisabled)
            {
                _logger.ServerDisabled();
                response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                
                if (serverSwitchContext.RetryAfter.HasValue)
                {
                    response.Headers[HeaderNames.RetryAfter] = serverSwitchContext.RetryAfter.Value.ToString(NumberFormatInfo.InvariantInfo);
                }
            }
            else
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
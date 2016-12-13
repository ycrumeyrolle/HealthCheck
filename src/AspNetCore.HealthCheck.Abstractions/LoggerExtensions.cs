using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AspNetCore.HealthCheck
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _serverDisabled;
        private static readonly Action<ILogger, IEnumerable<HealthCheckResult>, Exception> _healthCheckFailed;
        private static readonly Action<ILogger, Exception> _healthCheckSucceeded;

        static LoggerExtensions()
        {
            _serverDisabled = LoggerMessage.Define(LogLevel.Error, 0, "The server is disabled");
            _healthCheckFailed = LoggerMessage.Define<IEnumerable<HealthCheckResult>>(LogLevel.Error, 1, "Health check has failed : {errors}");
            _healthCheckSucceeded = LoggerMessage.Define(LogLevel.Debug, 2, "Health check has succeeded");
        }

        public static void ServerDisabled(this ILogger logger)
        {
            _serverDisabled(logger, null);
        }

        public static void HealthCheckFailed(this ILogger logger, HealthResponse response)
        {
            _healthCheckFailed(logger, response.Errors, null);
        }

        public static void HealthCheckSucceeded(this ILogger logger)
        {
            _healthCheckSucceeded(logger, null);
        }
    }
}

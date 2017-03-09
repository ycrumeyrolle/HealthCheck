using System;
using Microsoft.Extensions.Logging;

namespace AspNetCore.HealthCheck
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _serverDisabled;
        private static readonly Action<ILogger, HealthCheckResult, Exception> _healthCheckFailed;
        private static readonly Action<ILogger, HealthCheckResult, Exception> _healthCheckError;
        private static readonly Action<ILogger, Exception> _healthCheckSucceeded;

        private static readonly Action<ILogger, Exception> _authorizationFailed;

        static LoggerExtensions()
        {
            _serverDisabled = LoggerMessage.Define(LogLevel.Error, 0, "The server is disabled");
            _healthCheckFailed = LoggerMessage.Define<HealthCheckResult>(LogLevel.Error, 1, "Health check has failed : {result}");
            _healthCheckSucceeded = LoggerMessage.Define(LogLevel.Debug, 2, "Health check has succeeded");
            _healthCheckError = LoggerMessage.Define<HealthCheckResult>(LogLevel.Error, 3, "Health check error : {result}");

            _authorizationFailed = LoggerMessage.Define(LogLevel.Error, 4, "Authoriation has failed.");
     }

        public static void ServerDisabled(this ILogger logger)
        {
            _serverDisabled(logger, null);
        }

        public static void HealthCheckError(this ILogger logger, HealthCheckResult result, Exception error)
        {
            _healthCheckError(logger, result, error);
        }

        public static void HealthCheckFailed(this ILogger logger, HealthCheckResult result)
        {
            _healthCheckFailed(logger, result, null);
        }

        public static void HealthCheckSucceeded(this ILogger logger)
        {
            _healthCheckSucceeded(logger, null);
        }

        public static void AuthorizationFailed(this ILogger logger)
        {
            _authorizationFailed(logger, null);
        }
    }
}

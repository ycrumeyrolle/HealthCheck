using System;
using Microsoft.Extensions.Logging;

namespace AspNetCore.HealthCheck
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _serverDisabled = LoggerMessage.Define(LogLevel.Error, 0, "The server is disabled.");
        private static readonly Action<ILogger, HealthCheckResult, Exception> _healthCheckFailed = LoggerMessage.Define<HealthCheckResult>(LogLevel.Error, 1, "Health check has failed : {result}");
        private static readonly Action<ILogger, HealthCheckResult, Exception> _healthCheckError = LoggerMessage.Define<HealthCheckResult>(LogLevel.Error, 3, "Health check error : {result}");
        private static readonly Action<ILogger, Exception> _healthCheckSucceeded = LoggerMessage.Define(LogLevel.Debug, 2, "Health check has succeeded.");
        private static readonly Action<ILogger, string, Exception> _invalidPolicy = LoggerMessage.Define<string>(LogLevel.Debug, 4, "Policy '{policyName}' is invalid.");

        private static readonly Action<ILogger, Exception> _authorizationFailed = LoggerMessage.Define(LogLevel.Error, 5, "Authorization has failed.");

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
        
        public static void InvalidPolicy(this ILogger logger, string policyName)
        {
            _invalidPolicy(logger, policyName, null);
        }

        public static void AuthorizationFailed(this ILogger logger)
        {
            _authorizationFailed(logger, null);
        }
    }
}

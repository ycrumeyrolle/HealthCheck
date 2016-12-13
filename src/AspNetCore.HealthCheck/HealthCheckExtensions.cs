using System;
using AspNetCore.HealthCheck;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Sends request to remote server as specified in options
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">Options for setting port, host, and scheme</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            return app.UseMiddleware<HealthCheckMiddleware>(Options.Create(options));
        }
    }
}
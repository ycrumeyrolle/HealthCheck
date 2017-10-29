using System;
using AspNetCore.HealthCheck;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// IApplicationBuilder extensions HealthCheckMiddleware.
    /// </summary>
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Enable health check endpoint.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">Options for request path.</param>
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

        /// <summary>
        /// Enable health check endpoint.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path">Request path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var options = new HealthCheckOptions { Path = new PathString(path) };
            return app.UseHealthCheck(options);
        }
    }
}
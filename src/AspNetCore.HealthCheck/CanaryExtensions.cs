using System;
using AspNetCore.HealthCheck;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// IApplicationBuilder extensions HealthCheckMiddleware.
    /// </summary>
    public static class CanaryExtensions
    {
        /// <summary>
        /// Enable canary endpoint. 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">Options for request path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCanary(this IApplicationBuilder app, CanaryOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<CanaryMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// Enable canary endpoint.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path">Request path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCanary(this IApplicationBuilder app, string path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var options = new CanaryOptions { Path = new PathString(path) };

            return app.UseCanary(options);
        }
    }
}
using System;
using AspNetCore.HealthCheck.Http;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpWatchOptions options)
        {
            var settings = new HttpWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.Request);
            return builder.Add<HttpWatcher>(settings);
        }

        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, string name, Action<HttpHealthCheckBuilder> configureAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            var settingsBuilder = new HttpHealthCheckBuilder(name);
            configureAction(settingsBuilder);
            var settingsCollection = settingsBuilder.Build();

            for (int i = 0; i < settingsCollection.Requests.Count; i++)
            {
                var request = settingsCollection.Requests[i];
                var settingsName = settingsCollection.Requests.Count > 1 ? $"{settingsCollection.Name} {i+1}" : settingsCollection.Name;                
                var settings = new HttpWatchSettings(settingsName, settingsCollection.Critical, settingsCollection.Frequency, settingsCollection.Tags, request, settingsCollection.BeforeSend);
                builder.AddHttpEndpointCheck(settings);
            }

            return builder;
        }

        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new HttpWatchOptions();
            configuration.Bind(options);

            return builder.AddHttpEndpointCheck(options);
        }

        private static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpWatchSettings settings)
        {
            return builder.Add<HttpWatcher>(settings);
        }
    }
}
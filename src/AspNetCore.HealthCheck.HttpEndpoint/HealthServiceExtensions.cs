using System;
using AspNetCore.HealthCheck.HttpEndpoint;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpEndpointWatchOptions options)
        {
            var settings = new HttpEndpointWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.Request);
            return builder.Add<HttpEndpointWatcher>(settings);
        }

        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, string name, Action<HttpEndpointHealthCheckBuilder> configureAction)
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

            var settingsBuilder = new HttpEndpointHealthCheckBuilder(name);
            configureAction(settingsBuilder);
            var settingsCollection = settingsBuilder.Build();

            for (int i = 0; i < settingsCollection.Requests.Count; i++)
            {
                var request = settingsCollection.Requests[i];
                var settingsName = settingsCollection.Requests.Count > 1 ? $"{settingsCollection.Name} {i+1}" : settingsCollection.Name;                
                var settings = new HttpEndpointWatchSettings(settingsName, settingsCollection.Critical, settingsCollection.Frequency, settingsCollection.Tags, request, settingsCollection.BeforeSend);
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

            var options = new HttpEndpointWatchOptions();
            configuration.Bind(options);

            return builder.AddHttpEndpointCheck(options);
        }

        private static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpEndpointWatchSettings settings)
        {
            return builder.Add<HttpEndpointWatcher>(settings);
        }
    }
}
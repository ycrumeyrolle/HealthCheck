using System;
using AspNetCore.HealthCheck.Http;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheck
{
    public static class HttpHealthServiceExtensions
    {
        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpCheckOptions options)
        {
            var settings = new HttpCheckSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.Request);
            return builder.Add<HttpCheck>(settings);
        }

        public static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, string name, Action<HttpCheckSettingsBuilder> configureAction)
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

            var settingsBuilder = new HttpCheckSettingsBuilder(name);
            configureAction(settingsBuilder);
            var settingsCollection = settingsBuilder.Build();

            for (int i = 0; i < settingsCollection.Requests.Count; i++)
            {
                var request = settingsCollection.Requests[i];
                var settingsName = settingsCollection.Requests.Count > 1 ? $"{settingsCollection.Name} {i+1}" : settingsCollection.Name;                
                var settings = new HttpCheckSettings(settingsName, settingsCollection.Critical, settingsCollection.Frequency, settingsCollection.Tags, request, settingsCollection.BeforeSend);
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

            var options = new HttpCheckOptions();
            configuration.Bind(options);

            return builder.AddHttpEndpointCheck(options);
        }

        private static HealthCheckBuilder AddHttpEndpointCheck(this HealthCheckBuilder builder, HttpCheckSettings settings)
        {
            return builder.Add<HttpCheck>(settings);
        }
    }
}
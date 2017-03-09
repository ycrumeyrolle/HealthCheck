using System;
using AspNetCore.HealthCheck.Counter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class HealthServiceExtensions
    {
        public static HealthCheckBuilder AddCounterCheck(this HealthCheckBuilder builder, CounterWatchOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options== null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new CounterWatchSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ErrorThreshold, options.WarningThreshold, options.Distributed);
            return builder.AddCounterCheck(settings);
        }
        
        public static HealthCheckBuilder AddCounterCheck(this HealthCheckBuilder builder, string name, Action<CounterHealthCheckBuilder> configureAction)
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

            var settingsBuilder = new CounterHealthCheckBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddCounterCheck(settings);
        }

        public static HealthCheckBuilder AddCounterCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new CounterWatchOptions();
            configuration.Bind(options);

            return builder.AddCounterCheck(options);
        }

        private static HealthCheckBuilder AddCounterCheck(this HealthCheckBuilder builder, CounterWatchSettings settings)
        {
            builder.Services.AddLocalCounters();
            return builder.Add<CounterWatcher>(settings);
        }
    }
}
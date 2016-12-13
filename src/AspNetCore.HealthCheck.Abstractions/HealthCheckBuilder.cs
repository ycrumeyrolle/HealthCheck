using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckBuilder
    {
        /// <summary>
        /// Initializes a new <see cref = "HealthCheckBuilder"/> instance.
        /// </summary>
        /// <param name = "services">The <see cref = "IServiceCollection"/> to add services to.</param>
        public HealthCheckBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public HealthCheckBuilder Add<TWatcher>(IWatchSettings settings) where TWatcher : class, IHealthWatcher
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(settings.Name))
            {
                throw new ArgumentException($"Property {nameof(settings.Name)} cannot be null or empty.", nameof(settings));
            }
            
            Services.TryAddTransient<TWatcher>();
            Settings.Add(typeof(TWatcher), settings);
            return this;
        }

        /// <summary>
        /// Gets the <see cref = "IServiceCollection"/> where Health services are configured.
        /// </summary>
        public IServiceCollection Services { get; }

        public SettingsCollection Settings { get; } = new SettingsCollection();

        public HealthCheckPolicy Build()
        {
            return new HealthCheckPolicy(Settings);
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckBuilder
    {
        private int _millisecondsDelay = 2000;
        private int _frequency;

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

        public HealthCheckBuilder Add<TCheck>(IHealthCheckSettings settings) where TCheck : class, IHealthCheck
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(settings.Name))
            {
                throw new ArgumentException($"Property {nameof(settings.Name)} cannot be null or empty.", nameof(settings));
            }
            
            Services.TryAddTransient<TCheck>();
            Settings.Add(typeof(TCheck), settings);
            return this;
        }

        public HealthCheckBuilder SetDefaultTimeout(int millisecondsDelay)
        {
            _millisecondsDelay = millisecondsDelay;
            return this;
        }

        public HealthCheckBuilder SetDefaultFrequency(int frequency)
        {
            _frequency = frequency;
            return this;
        }

        /// <summary>
        /// Gets the <see cref = "IServiceCollection"/> where Health services are configured.
        /// </summary>
        public IServiceCollection Services { get; }

        public HealthCheckSettingsCollection Settings { get; } = new HealthCheckSettingsCollection();

        public HealthCheckPolicy Build()
        {
            for (int i = 0; i < Settings.Count; i++)
            {
                var setting = Settings[i].Value;
                if (setting.Timeout == 0)
                {
                    setting.Timeout = _millisecondsDelay;
                }

                if (setting.Frequency == 0)
                {
                    setting.Frequency = _frequency;
                }

                if (setting.Critical)
                {
                    setting.Tags.Add("critical");
                }
            }

            return new HealthCheckPolicy(Settings);
        }
    }
}
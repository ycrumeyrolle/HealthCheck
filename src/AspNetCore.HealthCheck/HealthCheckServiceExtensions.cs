using System;
using AspNetCore.HealthCheck;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HealthCheckServiceExtensions
    {
        private static IServiceCollection AddHealth(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.AddAuthorization();
            services.AddTransient<ISystemClock, SystemClock>();
            services.AddTransient<IHealthCheckService, DefaultHealthCheckService>();
            services.AddTransient<ICheckFactory, DefaultCheckFactory>();
            return services;
        }

        public static IServiceCollection AddHealth(this IServiceCollection services, Action<HealthCheckBuilder> configureBuilder)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHealth();
            var builder = new HealthCheckBuilder(services);
            configureBuilder(builder);
            services.AddSingleton<IHealthCheckPolicyProvider>(new DefaultHealthCheckPolicyProvider(builder.Build()));
            return services;
        }

        public static IServiceCollection AddServerFileSwitch(this IServiceCollection services, Action<ServerFileSwitchOptions> configureAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureAction == null)
            {
                throw new ArgumentNullException(nameof(configureAction));
            }

            services.AddOptions();
            services.AddTransient<IServerSwitch, FileServerSwitch>();
            services.Configure(configureAction);

            return services;
        }
    }
}
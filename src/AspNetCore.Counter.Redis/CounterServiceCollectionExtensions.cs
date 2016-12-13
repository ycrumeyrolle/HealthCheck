using System;
using AspNetCore.Counter;
using AspNetCore.Counter.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding counter services to the DI container.
    /// </summary>
    public static class CounterServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for using Redis counters.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddRedisCounters(this IServiceCollection services, Action<RedisCounterOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ICounterProvider), typeof(RedisCounterProvider)));
            services.Configure(setupAction);
            return services;
        }

        /// <summary>
        /// Adds services required for using Redis counters.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddRedisCounters(this IServiceCollection services, string configuration)
        {
            return services.AddRedisCounters(options =>
            {
                options.Configuration = configuration;
            });
        }
    }
}
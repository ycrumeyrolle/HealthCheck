using System;
using AspNetCore.Counter;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding counter services to the DI container.
    /// </summary>
    public static class CounterServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for using in-memory counters.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddLocalCounters(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Singleton(typeof(ICounterProvider), typeof(LocalCounterProvider)));
            return services;
        }
    }
}
using System;

namespace AspNetCore.Counter
{
    /// <summary>
    /// Extension methods <see cref="ICounterProvider"/>
    /// </summary>
    public static class CounterProviderExtensions
    {
        private static ICounter GetCounter<T>(this ICounterProvider provider, bool distributed)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetCounter(TypeNameHelper.GetTypeDisplayName(typeof(T)), distributed);
        }

        public static ICounter GetLocalCounter(this ICounterProvider provider, string name)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetCounter(name, distributed: false);
        }

        public static ICounter GetDistributedCounter(this ICounterProvider provider, string name)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetCounter(name, distributed: true);
        }

        public static ICounter GetLocalCounter<T>(this ICounterProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetCounter<T>(distributed: false);
        }

        public static ICounter GetDistributedCounter<T>(this ICounterProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetCounter<T>(distributed: true);
        }
    }
}
using System;
using AspNetCore.HealthCheck.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class HealthEntityFrameworkCoreServiceExtensions
    {
        public static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, EntityFrameworkCoreWatchOptions options) where TDbContext: DbContext
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new EntityFrameworkCoreWatchSettings<TDbContext>(options.Name, options.Critical, options.Frequency, options.Tags);
            return builder.AddEntityFrameworkCoreCheck<TDbContext>(settings);
        }

        public static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, string name, Action<EntityFrameworkCoreHealthCheckBuilder<TDbContext>> configureAction) where TDbContext : DbContext
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

            var settingsBuilder = new EntityFrameworkCoreHealthCheckBuilder<TDbContext>(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddEntityFrameworkCoreCheck(settings);
        }

        public static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, IConfiguration configuration) where TDbContext : DbContext
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new EntityFrameworkCoreWatchOptions();
            configuration.Bind(options);

            return builder.AddEntityFrameworkCoreCheck<TDbContext>(options);
        }

        private static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, EntityFrameworkCoreWatchSettings<TDbContext> settings) where TDbContext : DbContext
        {
            builder.Services.TryAddTransient(typeof(EntityFrameworkCoreWatcher<TDbContext>));
            return builder.Add<EntityFrameworkCoreWatcher<TDbContext>>(settings);
        }
    }
}
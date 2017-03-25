using System;
using AspNetCore.HealthCheck.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class HealthEntityFrameworkCoreServiceExtensions
    {
        public static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, EntityFrameworkCoreCheckOptions options) where TDbContext: DbContext
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new EntityFrameworkCoreCheckSettings<TDbContext>(options.Name, options.Critical, options.Frequency, options.Tags);
            return builder.AddEntityFrameworkCoreCheck<TDbContext>(settings);
        }

        public static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, string name, Action<EntityFrameworkCoreCheckSettingsBuilder<TDbContext>> configureAction) where TDbContext : DbContext
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

            var settingsBuilder = new EntityFrameworkCoreCheckSettingsBuilder<TDbContext>(name);
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

            var options = new EntityFrameworkCoreCheckOptions();
            configuration.Bind(options);

            return builder.AddEntityFrameworkCoreCheck<TDbContext>(options);
        }

        private static HealthCheckBuilder AddEntityFrameworkCoreCheck<TDbContext>(this HealthCheckBuilder builder, EntityFrameworkCoreCheckSettings<TDbContext> settings) where TDbContext : DbContext
        {
            builder.Services.TryAddTransient(typeof(EntityFrameworkCoreCheck<TDbContext>));
            return builder.Add<EntityFrameworkCoreCheck<TDbContext>>(settings);
        }
    }
}
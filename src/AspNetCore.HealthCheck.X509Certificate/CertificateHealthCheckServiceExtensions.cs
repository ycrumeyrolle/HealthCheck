﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.HealthCheck
{
    public static class CertificateHealthServiceExtensions
    {
        public static HealthCheckBuilder AddX509CertificateCheck(this HealthCheckBuilder builder, X509CertificateCheckOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = new X509CertificateCheckSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.Thumbprint, options.StoreName, options.StoreLocation, options.ExpirationOffsetInMinutes);
            return builder.AddX509CertificateCheck(settings);
        }

        public static HealthCheckBuilder AddX509CertificateCheck(this HealthCheckBuilder builder, string name, Action<X509CertificateCheckSettingsBuilder> configureAction)
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

            var settingsBuilder = new X509CertificateCheckSettingsBuilder(name);
            configureAction(settingsBuilder);
            var settings = settingsBuilder.Build();
            return builder.AddX509CertificateCheck(settings);
        }

        public static HealthCheckBuilder AddX509CertificateCheck(this HealthCheckBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new X509CertificateCheckOptions();
            configuration.Bind(options);

            return builder.AddX509CertificateCheck(options);
        }

        private static HealthCheckBuilder AddX509CertificateCheck(this HealthCheckBuilder builder, X509CertificateCheckSettings settings)
        {
            builder.Services.TryAddTransient<ICertificateResolver, CertificateResolver>();
            builder.Services.TryAddTransient(typeof(X509CertificateCheck));
            return builder.Add<X509CertificateCheck>(settings);
        }
    }
}
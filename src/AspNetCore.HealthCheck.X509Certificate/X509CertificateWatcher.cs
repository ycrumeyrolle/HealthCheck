using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateWatcher : HealthWatcher<X509CertificateWatchSettings>
    {
        private readonly ISystemClock _clock;
        private readonly ICertificateResolver _certificateResolver;

        public X509CertificateWatcher(ISystemClock clock, ICertificateResolver certificateResolver)
        {
            _clock = clock;
            _certificateResolver = certificateResolver;
        }

        public override Task CheckHealthAsync(HealthContext context, X509CertificateWatchSettings settings)
        {
            var certificate = _certificateResolver.ResolveCertificate(settings.StoreName, settings.StoreLocation, settings.Thumbprint);
            if (certificate == null)
            {
#if NETSTANDARD1_3
                // In NetStandard 1.3, we are no able to determinate if the certificate is missing or invalid.
                context.Fail("Missing or invalid certificate.");
#else
                context.Fail("Missing certificate.");
#endif
                return TaskCache.CompletedTask;
            }

#if !NETSTANDARD1_3
            if (!certificate.Verify())
            {
                context.Fail("Invalid certificate.");
                return TaskCache.CompletedTask;
            }
#endif

            if (certificate.NotAfter < _clock.UtcNow.AddMinutes(settings.ExpirationOffsetInMinutes))
            {
                context.Warn("Certificate will expire soon.");
                return TaskCache.CompletedTask;
            }

            context.Succeed();

            return TaskCache.CompletedTask;
        }
    }
}
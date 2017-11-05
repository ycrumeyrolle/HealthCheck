using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateCheck : HealthCheck<X509CertificateCheckSettings>
    {
        private readonly ISystemClock _clock;
        private readonly ICertificateResolver _certificateResolver;

        public X509CertificateCheck(ISystemClock clock, ICertificateResolver certificateResolver)
        {
            _clock = clock;
            _certificateResolver = certificateResolver;
        }

        public override Task CheckHealthAsync(HealthCheckContext context, X509CertificateCheckSettings settings)
        {
            var certificate = _certificateResolver.ResolveCertificate(settings.StoreName, settings.StoreLocation, settings.Thumbprint);
            if (certificate == null)
            {
#if NETSTANDARD2_0
                // In NetStandard 2.0, we are no able to determinate if the certificate is missing or invalid.
                context.Fail("Missing or invalid certificate.");
#else
                context.Fail("Missing certificate.");
#endif
                return TaskCache.CompletedTask;
            }

#if !NETSTANDARD2_0
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
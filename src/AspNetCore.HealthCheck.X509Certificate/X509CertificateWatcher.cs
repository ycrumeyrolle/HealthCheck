using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateWatcher : HealthWatcher<X509CertificateWatchSettings>
    {
        private readonly ISystemClock _clock;

        public X509CertificateWatcher(ISystemClock clock)
        {
            _clock = clock;
        }

        public override Task CheckHealthAsync(HealthContext context, X509CertificateWatchSettings settings)
        {
            var certificate = GetCertificateFromStore(settings.StoreName, settings.StoreLocation, settings.Thumbprint);
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

#if NETSTANDARD1_3
        private static X509Certificate2 GetCertificateFromStore(StoreName name, StoreLocation location, string thumbprint)
        {
            using (var store = new X509Store(name, location))
            {
                store.Open(OpenFlags.ReadOnly);
                var matchingCerts = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
                return (matchingCerts != null && matchingCerts.Count > 0) ? matchingCerts[0] : null;
            }
        }

#else
        private static X509Certificate2 GetCertificateFromStore(StoreName name, StoreLocation location, string thumbprint)
        {
            var store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var matchingCerts = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                return (matchingCerts != null && matchingCerts.Count > 0) ? matchingCerts[0] : null;
            }
            finally
            {
                store.Close();
            }
        }
#endif
    }
}
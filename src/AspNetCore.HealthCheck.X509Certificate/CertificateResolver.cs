using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public class CertificateResolver : ICertificateResolver
    {
        public X509Certificate2 ResolveCertificate(StoreName name, StoreLocation location, string thumbprint)
        {
#if NETSTANDARD2_0
            using (var store = new X509Store(name, location))
            {
                store.Open(OpenFlags.ReadOnly);
                var matchingCerts = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
                return (matchingCerts != null && matchingCerts.Count > 0) ? matchingCerts[0] : null;
            }

#else
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
#endif
        }
    }
}
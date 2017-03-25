using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateCheckSettings : HealthCheckSettings
    {
        public X509CertificateCheckSettings(string name,  bool critical, int frequency, IEnumerable<string> tags, string thumbprint, StoreName storeName, StoreLocation storeLocation, double expirationOffsetInMinutes)
            : base(name, critical, frequency, tags)
        {
            Thumbprint = thumbprint;
            StoreName = storeName;
            StoreLocation = storeLocation;
            ExpirationOffsetInMinutes = expirationOffsetInMinutes;
        }

        public string Thumbprint { get; }

        public StoreName StoreName { get; }

        public StoreLocation StoreLocation { get; }

        public double ExpirationOffsetInMinutes { get; internal set; }
    }
}
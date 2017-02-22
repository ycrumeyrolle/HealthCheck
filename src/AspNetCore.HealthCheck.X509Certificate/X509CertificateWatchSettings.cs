using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateWatchSettings : WatchSettings
    {
        public X509CertificateWatchSettings(string name,  bool critical, int frequency, IEnumerable<string> tags, string thumbprint, StoreName storeName, StoreLocation storeLocation)
            : base(name, critical, frequency, tags)
        {
            Thumbprint = thumbprint;
            StoreName = storeName;
            StoreLocation = storeLocation;
        }

        public string Thumbprint { get; }

        public StoreName StoreName { get; }

        public StoreLocation StoreLocation { get; }
    }
}
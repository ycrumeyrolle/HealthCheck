using System;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateHealthCheckBuilder : SettingsHealthCheckBuilder<X509CertificateWatchSettings>
    {
        private string _thumbprint;
        private StoreName _storeName;
        private StoreLocation _storeLocation;

        public X509CertificateHealthCheckBuilder(string name)
            : base(name)
        {
            _storeName = StoreName.My;
            _storeLocation = StoreLocation.LocalMachine;
        }

        public X509CertificateHealthCheckBuilder WithThumbprint(string thumbprint)
        {
            _thumbprint = thumbprint;
            return this;
        }

        public X509CertificateHealthCheckBuilder InStore(StoreName storeName, StoreLocation storeLocation)
        {
            _storeName = storeName;
            _storeLocation = storeLocation;
            return this;
        }

        public override X509CertificateWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_thumbprint))
            {
                throw new InvalidOperationException("No thumbprint is defined.");
            }
            
            return new X509CertificateWatchSettings(Name, Critical, Frequency, Tags, _thumbprint, _storeName, _storeLocation);
        }
    }
}
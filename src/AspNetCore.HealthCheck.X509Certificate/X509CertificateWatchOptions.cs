﻿using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public class X509CertificateWatchOptions : WatchOptions
    {
        public string Thumbprint { get; set; }

        public StoreName StoreName { get; set; }

        public StoreLocation StoreLocation { get; set; }
    }
}
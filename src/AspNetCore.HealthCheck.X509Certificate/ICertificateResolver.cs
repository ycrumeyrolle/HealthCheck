﻿using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.HealthCheck
{
    public interface ICertificateResolver
    {
        /// <summary>
        /// Locates an <see cref="X509Certificate2"/> given its thumbprint.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="thumbprint">The thumbprint (as a hex string) of the certificate to resolve.</param>
        /// <returns>The resolved <see cref="X509Certificate2"/>, or null if the certificate cannot be found.</returns>
        X509Certificate2 ResolveCertificate(StoreName name, StoreLocation location, string thumbprint);
    }
}
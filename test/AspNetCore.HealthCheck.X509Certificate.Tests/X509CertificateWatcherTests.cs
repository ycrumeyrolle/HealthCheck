using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.X509Certificate.Tests
{
    public class X509CertificateWatcherTests
    {
        [Fact]
        public async Task CheckHealthAsync_CertificateValid_CheckSucceeded()
        {
            var certificate = new X509Certificate2(TestResources.TestCertificatePath, "testPassword");
            var watcher = new X509CertificateWatcher(new TestClock(certificate.NotAfter.AddDays(-30)), new TestCertificateResolver(certificate));

            var settings = new X509CertificateWatchSettings("x509", false, 0, null, "thumbprint", StoreName.My, StoreLocation.CurrentUser, 1440.0);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_NearExpirationCertificate_CheckWarned()
        {
            var certificate = new X509Certificate2(TestResources.TestCertificatePath, "testPassword");
            var watcher = new X509CertificateWatcher(new TestClock(certificate.NotAfter.AddMinutes(30)), new TestCertificateResolver(certificate));

            var settings = new X509CertificateWatchSettings("x509", false, 0, null, "thumbprint", StoreName.My, StoreLocation.CurrentUser, 1440.0);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasWarned);
        }

        [Fact]
        public async Task CheckHealthAsync_ExpiredCertificate_CheckFailed()
        {
            var certificate = new X509Certificate2(TestResources.TestCertificatePath, "testPassword");
            var watcher = new X509CertificateWatcher(new TestClock(certificate.NotAfter.AddDays(30)), new TestCertificateResolver(certificate));

            var settings = new X509CertificateWatchSettings("x509", false, 0, null, "thumbprint", StoreName.My, StoreLocation.CurrentUser, 1440.0);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var certificateResolver = new Mock<ICertificateResolver>();
            certificateResolver
                .Setup(r => r.ResolveCertificate(It.IsAny<StoreName>(), It.IsAny<StoreLocation>(), It.IsAny<string>()))
                .Throws(new CryptographicException());
            var watcher = new X509CertificateWatcher(new TestClock(DateTime.UtcNow), certificateResolver.Object);

            var settings = new X509CertificateWatchSettings("x509", false, 0, null, "thumbprint", StoreName.My, StoreLocation.CurrentUser, 1440.0);
            var context = new HealthContext(settings);

            await Assert.ThrowsAsync<CryptographicException>(async () => await watcher.CheckHealthAsync(context, settings));
        }

        private class TestClock : ISystemClock
        {
            private readonly DateTimeOffset _now;

            public TestClock(DateTimeOffset now)
            {
                _now = now;
            }

            public DateTimeOffset UtcNow => _now;
        }

        private class TestCertificateResolver : ICertificateResolver
        {
            private readonly X509Certificate2 _certificate;

            public TestCertificateResolver(X509Certificate2 certificate)
            {
                _certificate = certificate;
            }

            public X509Certificate2 ResolveCertificate(StoreName name, StoreLocation location, string thumbprint) => _certificate;
        }
    }

    public static class TestResources
    {
        private static readonly string _testCertificatePath =
#if NET452
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResources", "testCert.pfx");
#else
            Path.Combine(AppContext.BaseDirectory, "TestResources", "testCert.pfx");
#endif

        public static string TestCertificatePath => _testCertificatePath;
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.Http.Tests
{
    public class HttpCheckTests
    {
        [Fact]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded()
        {
            var check = new HttpCheck();

            var httpSettings = new HttpRequestSettings() { HttpMethod = HttpMethod.Get, Uri = new Uri("http://localhost") };
            var settings = new HttpCheckSettings("http", false, 0, null, httpSettings);
            var context = new HealthCheckContext(settings);
            var httpHandler = new TestHttpMessageHandler();
            httpHandler.Sender = r => new HttpResponseMessage(HttpStatusCode.OK);
            settings.HttpHandler = httpHandler;

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_UnhealthyStatusCode_CheckFailed()
        {
            var check = new HttpCheck();

            var httpSettings = new HttpRequestSettings() { HttpMethod = HttpMethod.Get, Uri = new Uri("http://localhost") };
            var settings = new HttpCheckSettings("http", false, 0, null, httpSettings);
            var context = new HealthCheckContext(settings);
            var httpHandler = new TestHttpMessageHandler();
            httpHandler.Sender = r => new HttpResponseMessage(HttpStatusCode.BadRequest);
            settings.HttpHandler = httpHandler;

            await check.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var check = new HttpCheck();

            var httpSettings = new HttpRequestSettings() { HttpMethod = HttpMethod.Get, Uri = new Uri("http://localhost") };
            var settings = new HttpCheckSettings("http", false, 0, null, httpSettings);
            var context = new HealthCheckContext(settings);
            var httpHandler = new TestHttpMessageHandler();
            httpHandler.Sender = r => throw new HttpRequestException();
            settings.HttpHandler = httpHandler;

            await Assert.ThrowsAsync<HttpRequestException>(async () => await check.CheckHealthAsync(context, settings));
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage> Sender { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                if (Sender != null)
                {
                    return Task.FromResult(Sender(request));
                }

                return Task.FromResult<HttpResponseMessage>(null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.Tests
{
    public class CanaryMiddlewareTests
    {
        [Fact]
        public async void Invoke_WithNonMatchingPath_IgnoresRequest()
        {
            var contextMock = GetMockContext("/nonmatchingpath");
            RequestDelegate next = _ =>
            {
                return Task.FromResult<object>(null);
            };

            var options = new Mock<IOptions<CanaryOptions>>();
            options.SetupGet(o => o.Value)
                .Returns(new CanaryOptions() { Path = "/canary" });

            var loggerFactory = new LoggerFactory();
            var healthService = new Mock<IHealthCheckService>();
            healthService.Setup(s => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()))
                .ReturnsAsync(HealthResponse.Empty);

            var defaultPolicy = new HealthCheckPolicy(new SettingsCollection());
            var policyProvider = new DefaultHealthCheckPolicyProvider(defaultPolicy);

            var serverSwitch = new Mock<IServerSwitch>();
            serverSwitch.Setup(s => s.CheckServerStateAsync(It.IsAny<ServerSwitchContext>()));

            var canaryMiddleware = new CanaryMiddleware(next, options.Object, loggerFactory, healthService.Object, policyProvider, serverSwitch.Object);
            await canaryMiddleware.Invoke(contextMock.Object);

            healthService.Verify(s  => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()), Times.Never());
        }

        [Fact]
        public async void Invoke_ServerDisabled_Returns503()
        {
            var contextMock = GetMockContext("/canary");
            RequestDelegate next = _ =>
            {
                return Task.FromResult<object>(null);
            };

            var options = new Mock<IOptions<CanaryOptions>>();
            options.SetupGet(o => o.Value)
                .Returns(new CanaryOptions() { Path = "/canary" });

            var loggerFactory = new LoggerFactory();
            var healthService = new Mock<IHealthCheckService>();
            healthService.Setup(s => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()))
                .ReturnsAsync(HealthResponse.Empty);

            var defaultPolicy = new HealthCheckPolicy(new SettingsCollection());
            var policyProvider = new DefaultHealthCheckPolicyProvider(defaultPolicy);

            var serverSwitch = new Mock<IServerSwitch>();
            serverSwitch.Setup(s => s.CheckServerStateAsync(It.IsAny<ServerSwitchContext>()))
                .Callback<ServerSwitchContext>(c => c.Disable())
                .Returns(Task.FromResult(0));
            
            var canaryMiddleware = new CanaryMiddleware(next, options.Object, loggerFactory, healthService.Object, policyProvider, serverSwitch.Object);
            await canaryMiddleware.Invoke(contextMock.Object);


            serverSwitch.Verify(s => s.CheckServerStateAsync(It.IsAny<ServerSwitchContext>()), Times.Once());
            healthService.Verify(s => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()), Times.Never());
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, contextMock.Object.Response.StatusCode);
        }

        [Fact]
        public async void Invoke_ServerEnabled_Returns200()
        {
            var contextMock = GetMockContext("/canary");
            RequestDelegate next = _ =>
            {
                return Task.FromResult<object>(null);
            };

            var options = new Mock<IOptions<CanaryOptions>>();
            options.SetupGet(o => o.Value)
                .Returns(new CanaryOptions() { Path = "/canary" });

            var loggerFactory = new LoggerFactory();
            var healthService = new Mock<IHealthCheckService>();
            healthService.Setup(s => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()))
                .ReturnsAsync(new HealthResponse(new List<HealthCheckResult> { new HealthCheckResult { Status = HealthStatus.OK } }));

            var defaultPolicy = new HealthCheckPolicy(new SettingsCollection());
            var policyProvider = new DefaultHealthCheckPolicyProvider(defaultPolicy);

            var serverSwitch = new Mock<IServerSwitch>();
            serverSwitch.Setup(s => s.CheckServerStateAsync(It.IsAny<ServerSwitchContext>()))
                .Returns(Task.FromResult(0));

            var canaryMiddleware = new CanaryMiddleware(next, options.Object, loggerFactory, healthService.Object, policyProvider, serverSwitch.Object);
            await canaryMiddleware.Invoke(contextMock.Object);

            serverSwitch.Verify(s => s.CheckServerStateAsync(It.IsAny<ServerSwitchContext>()), Times.Once());
            healthService.Verify(s => s.CheckHealthAsync(It.IsAny<HealthCheckPolicy>()), Times.Once());
            Assert.Equal(StatusCodes.Status200OK, contextMock.Object.Response.StatusCode);
        }

        private Mock<HttpContext> GetMockContext(string path)
        {
            var contextMock = new Mock<HttpContext>(MockBehavior.Strict);
            contextMock.SetupAllProperties();
            contextMock
                .SetupGet(c => c.Request.Path)
                .Returns(new PathString(path));
            contextMock
                .SetupGet(c => c.Request.Host)
                .Returns(new HostString("localhost"));
            contextMock
                .SetupGet(c => c.Request.ContentType)
                .Returns("");
            contextMock
                .SetupGet(c => c.Request.Scheme)
                .Returns("http");
            contextMock
                .SetupGet(c => c.Response.Headers)
                .Returns(new Mock<IHeaderDictionary>().Object);
            contextMock
                .SetupGet(c => c.Response.Body)
                .Returns(new Mock<Stream>().Object);
            contextMock
                .SetupGet(c => c.User)
                .Returns(new ClaimsPrincipal());
            contextMock
                .SetupGet(c => c.Request.Method)
                .Returns("GET");
            contextMock
                .SetupGet(c => c.Request.Protocol)
                .Returns("HTTP/1.1");
            contextMock
                .SetupGet(c => c.Request.Headers)
                .Returns(new Mock<IHeaderDictionary>().Object);
            contextMock
                .SetupGet(c => c.Request.QueryString)
                .Returns(new QueryString());
            contextMock
                .SetupGet(c => c.Request.Query)
                .Returns(new Mock<IQueryCollection>().Object);
            contextMock
                .SetupGet(c => c.Request.Cookies)
                .Returns(new Mock<IRequestCookieCollection>().Object);
            contextMock
                .Setup(c => c.Request.ReadFormAsync(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.FromResult(new Mock<IFormCollection>().Object));
            contextMock
                .Setup(c => c.Request.HasFormContentType)
                .Returns(true);
            return contextMock;
        }

        private static TestServer CreateServer(HealthCheckOptions options, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    if (options != null)
                    {
                        app.UseHealthCheck(options);
                    }                    
                })
                .ConfigureServices(services => services.AddHealth(b => { }));

            return new TestServer(builder);
        }
    }
}

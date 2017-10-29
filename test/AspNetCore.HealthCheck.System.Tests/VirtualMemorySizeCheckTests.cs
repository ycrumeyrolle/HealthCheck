using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.System.Tests
{
    public class VirtualMemorySizeCheckTests
    {
        [Theory]
        [MemberData("GetSuccessThresholds")]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded(long virtualMemorySize, long errorThreshold, long warningThreshold)
        {
            var virtualMemorySizeProvider = new TestVirtualMemorySizeProvider(virtualMemorySize);
            var check = new VirtualMemorySizeCheck(virtualMemorySizeProvider);

            var settings = new FloorThresholdCheckSettings("disk space", false, 0, null, errorThreshold, warningThreshold);
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetFailureThresholds")]
        public async Task CheckHealthAsync_Unhealthy_CheckFailed(long virtualMemorySize, long errorThreshold, long warningThreshold)
        {
            var virtualMemorySizeProvider = new TestVirtualMemorySizeProvider(virtualMemorySize);
            var check = new VirtualMemorySizeCheck(virtualMemorySizeProvider);

            var settings = new FloorThresholdCheckSettings("virtual memory size", false, 0, null, errorThreshold, warningThreshold);
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetWarningThresholds")]
        public async Task CheckHealthAsync_Warning_CheckWarned(long virtualMemorySize, long errorThreshold, long warningThreshold)
        {
            var virtualMemorySizeProvider = new TestVirtualMemorySizeProvider(virtualMemorySize);
            var check = new VirtualMemorySizeCheck(virtualMemorySizeProvider);

            var settings = new FloorThresholdCheckSettings("virtual memory size", false, 0, null, errorThreshold, warningThreshold);
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasWarned);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var virtualMemorySizeProvider = new Mock<IVirtualMemorySizeProvider>();
            virtualMemorySizeProvider
                .Setup(p => p.GetVirtualMemorySize())
                .Throws(new Exception());
            var check = new VirtualMemorySizeCheck(virtualMemorySizeProvider.Object);

            var settings = new FloorThresholdCheckSettings("virtual memory size", false, 0, null, 0, 0);
            var context = new HealthCheckContext(settings);

            await Assert.ThrowsAsync<Exception>(async () => await check.CheckHealthAsync(context, settings));
        }

        public static IEnumerable<object[]> GetSuccessThresholds()
        {
            yield return new object[] { 1024, 0, 512 };
            yield return new object[] { 1024, 256, 512 };
            yield return new object[] { 1024, 256, 0 };
        }

        public static IEnumerable<object[]> GetFailureThresholds()
        {
            yield return new object[] { 1024, 1024, 2048 };
            yield return new object[] { 1024, 1024, 1025 };
            yield return new object[] { 1024, 1024, 0 };
            yield return new object[] { 0, 1024, 0 };
        }
        public static IEnumerable<object[]> GetWarningThresholds()
        {
            yield return new object[] { 1024, 0, 1024 };
            yield return new object[] { 1024, 256, 1024 };
            yield return new object[] { 512, 256, 1024 };
        }

        private class TestVirtualMemorySizeProvider : IVirtualMemorySizeProvider
        {
            private readonly long _virtualMemorySize;

            public TestVirtualMemorySizeProvider(long virtualMemorySize)
            {
                _virtualMemorySize = virtualMemorySize;
            }

            public long GetVirtualMemorySize() => _virtualMemorySize;
        }
    }
}

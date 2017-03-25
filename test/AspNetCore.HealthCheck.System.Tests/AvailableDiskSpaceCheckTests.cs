using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.System.Tests
{
    public class AvailableDiskSpaceCheckTests
    {
        [Theory]
        [MemberData("GetSuccessThresholds")]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var check = new AvailableDiskSpaceCheck(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceCheckSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetFailureThresholds")]
        public async Task CheckHealthAsync_Unhealthy_CheckFailed(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var check = new AvailableDiskSpaceCheck(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceCheckSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetWarningThresholds")]
        public async Task CheckHealthAsync_Warning_CheckWarned(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var check = new AvailableDiskSpaceCheck(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceCheckSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthCheckContext(settings);

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasWarned);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var availableFreeSpaceProvider = new Mock<IAvailableSpaceProvider>();
            availableFreeSpaceProvider
                .Setup(p => p.GetAvailableDiskSpace(It.IsAny<string>()))
                .Throws(new Exception());
            var check = new AvailableDiskSpaceCheck(availableFreeSpaceProvider.Object);

            var settings = new AvailableDiskSpaceCheckSettings("disk space", false, 0, null, 0, 0, "c");
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

        private class TestFreeSpaceProvider : IAvailableSpaceProvider
        {
            private readonly long _availableFreeSpace;

            public TestFreeSpaceProvider(long availableFreeSpace)
            {
                _availableFreeSpace = availableFreeSpace;
            }
            public long GetAvailableDiskSpace(string drive) => _availableFreeSpace;
        }
    }
}

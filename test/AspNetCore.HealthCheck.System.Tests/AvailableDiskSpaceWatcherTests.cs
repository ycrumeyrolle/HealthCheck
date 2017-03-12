using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace AspNetCore.HealthCheck.System.Tests
{
    public class AvailableDiskSpaceWatcherTests
    {
        [Theory]
        [MemberData("GetSuccessThresholds")]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var watcher = new AvailableDiskSpaceWatcher(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceWatchSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetFailureThresholds")]
        public async Task CheckHealthAsync_Unhealthy_CheckFailed(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var watcher = new AvailableDiskSpaceWatcher(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceWatchSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetWarningThresholds")]
        public async Task CheckHealthAsync_Warning_CheckWarned(long availableFreeSpace, long errorThreshold, long warningThreshold)
        {
            var availableFreeSpaceProvider = new TestFreeSpaceProvider(availableFreeSpace);
            var watcher = new AvailableDiskSpaceWatcher(availableFreeSpaceProvider);

            var settings = new AvailableDiskSpaceWatchSettings("disk space", false, 0, null, errorThreshold, warningThreshold, "c");
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasWarned);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var availableFreeSpaceProvider = new Mock<IFreeSpaceProvider>();
            availableFreeSpaceProvider
                .Setup(p => p.GetAvailableFreeSpace(It.IsAny<string>()))
                .Throws(new Exception());
            var watcher = new AvailableDiskSpaceWatcher(availableFreeSpaceProvider.Object);

            var settings = new AvailableDiskSpaceWatchSettings("disk space", false, 0, null, 0, 0, "c");
            var context = new HealthContext(settings);

            await Assert.ThrowsAsync<Exception>(async () => await watcher.CheckHealthAsync(context, settings));
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

        private class TestFreeSpaceProvider : IFreeSpaceProvider
        {
            private readonly long _availableFreeSpace;

            public TestFreeSpaceProvider(long availableFreeSpace)
            {
                _availableFreeSpace = availableFreeSpace;
            }
            public long GetAvailableFreeSpace(string drive) => _availableFreeSpace;
        }
    }
}

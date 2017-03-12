using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.HealthCheck.Counter;
using Moq;
using Xunit;
using AspNetCore.Counter;
using System.Collections.Generic;

namespace AspNetCore.HealthCheck.Counter.Tests
{
    public class CounterWatcherTests
    {
        [Theory]
        [MemberData("GetSuccessThresholds")]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded(long counterValue, long errorThreshold, long warningThreshold)
        {
            var counter = new LocalCounter("test");
            counter.Value = counterValue;
            var counterProvider = new TestCounterProvider(counter);
            var watcher = new CounterWatcher(counterProvider);

            var settings = new CounterWatchSettings("counter", false, 0, null, errorThreshold, warningThreshold, false);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetFailureThresholds")]
        public async Task CheckHealthAsync_Unhealthy_CheckFailed(long counterValue, long errorThreshold, long warningThreshold)
        {
            var counter = new LocalCounter("test");
            counter.Value = counterValue;
            var counterProvider = new TestCounterProvider(counter);
            var watcher = new CounterWatcher(counterProvider);

            var settings = new CounterWatchSettings("counter", false, 0, null, errorThreshold, warningThreshold, false);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.False(context.HasSucceeded);
        }

        [Theory]
        [MemberData("GetWarningThresholds")]
        public async Task CheckHealthAsync_Warning_CheckWarned(long counterValue, long errorThreshold, long warningThreshold)
        {
            var counter = new LocalCounter("test");
            counter.Value = counterValue;
            var counterProvider = new TestCounterProvider(counter);
            var watcher = new CounterWatcher(counterProvider);

            var settings = new CounterWatchSettings("counter", false, 0, null, errorThreshold, warningThreshold, false);
            var context = new HealthContext(settings);

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasWarned);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowsException()
        {
            var counterProvider = new Mock<ICounterProvider>();
            counterProvider
                .Setup(p => p.GetCounter(It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new Exception());
            var watcher = new CounterWatcher(counterProvider.Object);

            var settings = new CounterWatchSettings("counter", false, 0, null, 0, 0, false);
            var context = new HealthContext(settings);

            await Assert.ThrowsAsync<Exception>(async () => await watcher.CheckHealthAsync(context, settings));
        }

        public static IEnumerable<object[]> GetSuccessThresholds()
        {
            yield return new object[] { 0, 10, 5 };
            yield return new object[] { 1, 10, 5 };
            yield return new object[] { 4, 10, 5 };
        }
        public static IEnumerable<object[]> GetFailureThresholds()
        {
            yield return new object[] { 1, 0, 0 };
            yield return new object[] { 10, 10, 5 };
            yield return new object[] { 11, 10, 5 };
            yield return new object[] { 11, 10, 0 };
        }
        public static IEnumerable<object[]> GetWarningThresholds()
        {
            yield return new object[] { 5, 10, 5 };
            yield return new object[] { 6, 10, 5 };
            yield return new object[] { 9, 10, 5 };
        }

        private class TestCounterProvider : ICounterProvider
        {
            private readonly ICounter _counter;

            public TestCounterProvider(ICounter counter)
            {
                _counter = counter;
            }
            public ICounter GetCounter(string name, bool distributed)
            {
                return _counter;
            }
        }
    }
}

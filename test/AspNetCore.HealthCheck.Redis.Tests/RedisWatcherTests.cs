using System;
using System.Threading.Tasks;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace AspNetCore.HealthCheck.Redis.Tests
{
    public class RedisWatcherTests
    {
        [Fact]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded()
        {
            var watcher = new RedisWatcher();

            var settings = new RedisWatchSettings("redis", false, 0, null, "instance");
            var context = new HealthContext(settings);
            var database = new Mock<IDatabase>();
            database
                .Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
                .ReturnsAsync(TimeSpan.Zero);
            settings.Database = database.Object;

            await watcher.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowException()
        {
            var watcher = new RedisWatcher();

            var settings = new RedisWatchSettings("redis", false, 0, null, "instance");
            var context = new HealthContext(settings);

            var database = new Mock<IDatabase>();
            database
                .Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
                .ThrowsAsync(new Exception());
            settings.Database = database.Object;

            await Assert.ThrowsAsync<Exception>(async () => await watcher.CheckHealthAsync(context, settings));
        }
    }
}

using System;
using System.Threading.Tasks;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace AspNetCore.HealthCheck.Redis.Tests
{
    public class RedisCheckTests
    {
        [Fact]
        public async Task CheckHealthAsync_Healthy_CheckSucceeded()
        {
            var check = new RedisCheck();

            var settings = new RedisCheckSettings("redis", false, 0, null, "instance");
            var context = new HealthCheckContext(settings);
            var database = new Mock<IDatabase>();
            database
                .Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
                .ReturnsAsync(TimeSpan.Zero);
            settings.Database = database.Object;

            await check.CheckHealthAsync(context, settings);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task CheckHealthAsync_Unhealthy_ThrowException()
        {
            var check = new RedisCheck();

            var settings = new RedisCheckSettings("redis", false, 0, null, "instance");
            var context = new HealthCheckContext(settings);

            var database = new Mock<IDatabase>();
            database
                .Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
                .ThrowsAsync(new Exception());
            settings.Database = database.Object;

            await Assert.ThrowsAsync<Exception>(async () => await check.CheckHealthAsync(context, settings));
        }
    }
}

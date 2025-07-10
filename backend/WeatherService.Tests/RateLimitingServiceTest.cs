using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using WeatherService.Services.RateLimiting;

namespace WeatherService.Tests;

public class RateLimitingServiceTest
{
    private static RateLimitingService CreateService(int retriesRemaining = 5)
    {
        // I understand contention around the Moq library, however I wanted quick wins
        var multiplexerMock = new Mock<IConnectionMultiplexer>();

        var dbMock = new Mock<IDatabase>();
        dbMock.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);
        dbMock.Setup(db => db.StringDecrementAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(retriesRemaining);

        multiplexerMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(dbMock.Object);

        var options = Options.Create(new RateLimitingConfiguration
        {
            TimeoutMinutes = 60,
            RetryLimit = 5
        });

        return new RateLimitingService(options, multiplexerMock.Object);
    }

    [Fact]
    public async Task CheckRateLimitingAsync_ThrowsNoException_WhenUserHasRetriesRemaining()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert: If an exception is thrown, the test will fail.
        await service.CheckRateLimitingAsync("abc123");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CheckRateLimitingAsync_ThrowsException_WhenUserHasNoRetriesRemaining(int retriesRemaining)
    {
        // Arrange
        var service = CreateService(retriesRemaining);

        // Act & Assert
        await Assert.ThrowsAsync<RateLimitExceededException>(async () =>
        {
            await service.CheckRateLimitingAsync("abc123");
        });
    }
}
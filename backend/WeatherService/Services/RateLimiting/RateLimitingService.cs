using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace WeatherService.Services.RateLimiting;

/// <inheritdoc />
public class RateLimitingService : IRateLimitingService
{
    private const int DEFAULT_TIMEOUT_MINUTES = 60;
    private const int DEFAULT_TIMEOUT_CAPACITY = 5;

    private readonly IDatabase _redisDb;
    private readonly TimeSpan _timeoutDuration;
    private readonly int _retryLimit;

    public RateLimitingService(IOptions<RateLimitingConfiguration> rateLimitingConfiguration,
        IConnectionMultiplexer redis)
    {
        var timeoutMinutes = rateLimitingConfiguration.Value.TimeoutMinutes ?? DEFAULT_TIMEOUT_MINUTES;
        var retryLimit = rateLimitingConfiguration.Value.RetryLimit ?? DEFAULT_TIMEOUT_CAPACITY;

        _redisDb = redis.GetDatabase();
        _timeoutDuration = TimeSpan.FromMinutes(timeoutMinutes);
        _retryLimit = retryLimit;
    }

    /// <inheritdoc />
    public async Task CheckRateLimitingAsync(string userKey)
    {
        // Prefix the user key, to avoid key collisions
        var prefixedUserKey = $"U:{userKey}";

        // Ensure the default timeout is set for this user.
        // This will not overwrite an existing key.
        if (await _redisDb.StringSetAsync(prefixedUserKey, _retryLimit, _timeoutDuration, When.NotExists))
        {
            // If this operation returns true, the key was just created
            return;
        }

        // Get the current number of retries left for this user
        // This does not update the key expiry time
        var retriesLeft = await _redisDb.StringDecrementAsync(prefixedUserKey);

        // If the user has no retries left, throw an exception
        if (retriesLeft <= 0)
        {
            throw new RateLimitExceededException();
        }
    }
}
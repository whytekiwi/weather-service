
namespace WeatherService.Services.RateLimiting;

/// <summary>
/// Service for checking and enforcing rate limiting for users.
/// </summary>
public interface IRateLimitingService
{
    /// <summary>
    /// Checks if the user has exceeded their rate limit.
    /// If the user has exceeded their limit, an exception is thrown.
    /// </summary>
    /// <param name="userKey">The unique key for the user</param>
    /// <exception cref="RateLimitExceededException">Thrown when the user has exceeded their rate limit</exception>
    Task CheckRateLimitingAsync(string userKey);
}
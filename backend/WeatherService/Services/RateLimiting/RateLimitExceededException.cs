namespace WeatherService.Services.RateLimiting;

/// <summary>
/// An exception that we can throw when the rate limit is exceeded.
/// </summary>
public class RateLimitExceededException : Exception;
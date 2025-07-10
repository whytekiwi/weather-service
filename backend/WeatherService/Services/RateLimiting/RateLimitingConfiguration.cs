namespace WeatherService.Services.RateLimiting;

/// <summary>
/// Allows for configuration the rate limiting service without changing application code.
/// </summary>
public class RateLimitingConfiguration
{
    /// <summary>
    /// How long we apply rate limiting for a user, in minutes.
    /// </summary>
    public int? TimeoutMinutes { get; set; }

    /// <summary>
    /// How many requests a user can make in the timeout period.
    /// </summary>
    public int? RetryLimit { get; set; }
}
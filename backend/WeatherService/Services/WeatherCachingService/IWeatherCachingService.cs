using WeatherService.Models;

namespace WeatherService.Services.WeatherCachingService;

/// <summary>
/// Service for interacting with our cache of weather results
/// </summary>
/// <remarks>While we have redis, lets use it to save costs on calling third party apis</remarks>
public interface IWeatherCachingService
{
    /// <summary>
    /// Fetch an existing record from our cache.
    /// </summary>
    /// <param name="city">The city requested.</param>
    /// <param name="country">The country requested.</param>
    /// <returns>The <see cref="WeatherApiResult"/> object, if it exists. <c>null</c> if not.</returns>
    Task<WeatherApiResult?> CheckCacheAsync(string city, string country);

    /// <summary>
    /// Add a weather result to our cache, overwriting any existing data.
    /// </summary>
    /// <param name="city">The city requested.</param>
    /// <param name="country">The country requested.</param>
    /// <param name="weather">The weather result to cache.</param>
    Task AddToCacheAsync(string city, string country, WeatherApiResult weather);
}
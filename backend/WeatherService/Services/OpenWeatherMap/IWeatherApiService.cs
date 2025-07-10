using WeatherService.Models;

namespace WeatherService.Services.OpenWeatherMap;

/// <summary>
/// Service for querying weather data
/// </summary>
public interface IWeatherApiService
{
    /// <summary>
    /// Gets the weather for a given city and country.
    /// </summary>
    /// <param name="city">The name of the city to retrieve weather data for.</param>
    /// <param name="country">The country code (e.g., "US", "AU") corresponding to the city.</param>
    /// <param name="cancellationToken">Allows for early cancellation of asynchronous tasks.</param>
    /// <returns>A <see cref="WeatherApiResult"/> object with the weather data, or <c>null</c> if the data could not be retrieved.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the API response cannot be deserialized.</exception>
    Task<WeatherApiResult?> GetWeatherAsync(string city, string country, CancellationToken cancellationToken = default);
}
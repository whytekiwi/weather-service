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
    /// <returns>A <see cref="WeatherApiResult"/> object with the weather data, or <c>null</c> if the data could not be retrieved.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the API response cannot be deserialized.</exception>
    Task<WeatherApiResult?> GetWeather(string city, string country);
}
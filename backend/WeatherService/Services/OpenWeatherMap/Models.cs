using WeatherService.Models;

// Place these models here to prevent polluting the main namespace
namespace WeatherService.Services.OpenWeatherMap;

/// <summary>
/// Represents the result of a call to the OpenWeatherMap API.
/// </summary>
/// <param name="Weather">The weather array from the call</param>
public record ApiResult(Weather[]? Weather);

/// <summary>
/// Represents a single weather condition.
/// </summary>
/// <param name="Description">The description of the weather condition</param>
public record Weather(string? Description);

/// <summary>
/// Maps the OpenWeatherMap API result to the expected format.
/// </summary>
public static class ModelMapper
{
    /// <summary>
    /// Map a <see cref="WeatherApiResult">WeatherApiResult</see> from the OpenWeatherMap API.
    /// </summary>
    /// <param name="weather">The result from the OpenWeatherMap API.</param>
    /// <returns>A <see cref="WeatherApiResult"/> object if weather was found, otherwise <c>null</c>.</returns>
    /// <remarks>A very simple mapper that simply fetches the first weather condition's description.</remarks>
    public static WeatherApiResult? MapToApiResult(this ApiResult? weather)
    {
        if (weather?.Weather is null) return null;
        if (weather.Weather.Length == 0) return null;

        return new WeatherApiResult(weather.Weather[0].Description);
    }
}
namespace WeatherService.Services.OpenWeatherMap;

/// <summary>
/// The application configuration for the OpenWeatherMap service.
/// This class holds the API key and the base URL for the OpenWeatherMap API.
/// </summary>
/// <remarks>
/// Used on application startup to configure the OpenWeatherMap service.
/// Requires mutable properties to allow configuration via dependency injection.
/// </remarks>
public class OpenWeatherMapConfiguration
{
    public string? ApiKey { get; set; }
    public string? Url { get; set; }
}
namespace WeatherService.Models;

/// <summary>
/// The result sent to the client when calling the "weather" endpoint
/// </summary>
/// <param name="Description">The description of the weather in the requested city</param>
public record WeatherApiResult(string? Description);
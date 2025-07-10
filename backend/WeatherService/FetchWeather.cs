using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherService.Services.OpenWeatherMap;
using WeatherService.Services.RateLimiting;

namespace WeatherService;

/// <summary>
/// Function to fetch weather information.
/// </summary>
/// <param name="logger">Allows for logging of application information</param>
/// <param name="weatherApiService">Used for fetching </param>
public class FetchWeather(
    ILogger<FetchWeather> logger,
    IWeatherApiService weatherApiService,
    IRateLimitingService rateLimitingService)
{
    /// <summary>
    /// Fetch weather function
    /// </summary>
    /// <param name="req">The incoming HTTP request</param>
    /// <returns></returns>
    [Function("FetchWeather")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        // TODO: api key validation
        string userKey = "abc123";

        // A consideration for an orchestrator pattern could be made here, but we don't want to over-engineer
        try
        {
            // If needed, it would be simple to add a return value here to let the client know how many requests they have left
            await rateLimitingService.CheckRateLimitingAsync(userKey);
        }
        catch (RateLimitExceededException)
        {
            return new StatusCodeResult(StatusCodes.Status429TooManyRequests);
        }

        string? country = req.Query["country"];
        if (string.IsNullOrWhiteSpace(country))
        {
            return new BadRequestObjectResult("Please enter country query parameter");
        }

        string? city = req.Query["city"];
        if (string.IsNullOrWhiteSpace(city))
        {
            return new BadRequestObjectResult("Please enter city query parameter");
        }


        // TODO: check redis

        try
        {
            var result = await weatherApiService.GetWeatherAsync(city, country);

            if (string.IsNullOrWhiteSpace(result?.Description))
            {
                return new NoContentResult();
            }

            // TODO: store result in redis

            // While the requirements were to simply return "description", I use a structured object for extensibility
            return new OkObjectResult(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new NotFoundResult();
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
        {
            // Ensure we have valid logs
            logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult($"Error fetching weather for {city}, {country}");
        }
    }
}
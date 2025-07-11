using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherService.Services.ApiKeyValidation;
using WeatherService.Services.OpenWeatherMap;
using WeatherService.Services.RateLimiting;
using WeatherService.Services.WeatherCachingService;

namespace WeatherService;

/// <summary>
/// Function to fetch weather information.
/// </summary>
/// <param name="logger">Allows for logging of application information</param>
/// <param name="weatherApiService">Used for fetching </param>
public class FetchWeather(
    ILogger<FetchWeather> logger,
    IWeatherApiService weatherApiService,
    IRateLimitingService rateLimitingService,
    IApiKeyValidationService apiKeyValidationService,
    IWeatherCachingService weatherCachingService)
{
    /// <summary>
    /// Fetch weather function
    /// </summary>
    /// <param name="req">The incoming HTTP request</param>
    /// <returns></returns>
    [Function("Weather")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        // A consideration for an orchestrator pattern could be made here, but we don't want to over-engineer
        string userKey;
        try
        {
            userKey = apiKeyValidationService.ValidateApiKey(req);
        }
        catch (ApiKeyValidationException)
        {
            return new UnauthorizedResult();
        }

        try
        {
            // If needed, it would be simple to add a return value here to let the client know how many requests they have left
            await rateLimitingService.CheckRateLimitingAsync(userKey);
        }
        catch (RateLimitExceededException)
        {
            return new StatusCodeResult(StatusCodes.Status429TooManyRequests);
        }

        var country = req.Query["country"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(country))
        {
            return new BadRequestObjectResult("Please enter country query parameter");
        }

        var city = req.Query["city"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(city))
        {
            return new BadRequestObjectResult("Please enter city query parameter");
        }

        try
        {
            var weather = await weatherCachingService.CheckCacheAsync(city, country);
            if (weather != null)
                return new OkObjectResult(weather);
        }
        catch (JsonException ex)
        {
            // We should log the error, and fetch from the API again
            logger.LogError(ex, ex.Message);
        }

        try
        {
            var result = await weatherApiService.GetWeatherAsync(city, country);

            if (string.IsNullOrWhiteSpace(result?.Description))
            {
                return new NoContentResult();
            }

            await weatherCachingService.AddToCacheAsync(city, country, result);

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
            return new ObjectResult($"Error fetching weather for {city}, {country}")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
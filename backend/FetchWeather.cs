using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace WeatherService;

public class FetchWeather
{
    private readonly ILogger<FetchWeather> _logger;

    public FetchWeather(ILogger<FetchWeather> logger)
    {
        _logger = logger;
    }

    [Function("FetchWeather")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
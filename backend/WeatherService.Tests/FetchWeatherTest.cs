using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherService.Models;
using WeatherService.Services.ApiKeyValidation;
using WeatherService.Services.OpenWeatherMap;
using WeatherService.Services.RateLimiting;
using WeatherService.Services.WeatherCachingService;

namespace WeatherService.Tests;

public class FetchWeatherTests
{
    private readonly Mock<ILogger<FetchWeather>> _logger = new();
    private readonly Mock<IWeatherApiService> _weatherApiService = new();
    private readonly Mock<IRateLimitingService> _rateLimitingService = new();
    private readonly Mock<IApiKeyValidationService> _apiKeyValidationService = new();
    private readonly Mock<IWeatherCachingService> _weatherCachingService = new();

    private FetchWeather CreateFunction() =>
        new(
            _logger.Object,
            _weatherApiService.Object,
            _rateLimitingService.Object,
            _apiKeyValidationService.Object,
            _weatherCachingService.Object);

    private static DefaultHttpContext CreateHttpContext(string? country = "AU",
        string? city = "Melbourne")
    {
        return new DefaultHttpContext
        {
            Request =
            {
                QueryString = new QueryString($"?country={country}&city={city}")
            }
        };
    }

    [Fact]
    public async Task ReturnsUnauthorized_WhenApiKeyInvalid()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Throws(new ApiKeyValidationException());

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Returns429_WhenRateLimitExceeded()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .ThrowsAsync(new RateLimitExceededException());

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status429TooManyRequests, statusResult.StatusCode);
    }

    [Theory]
    [InlineData(null, "Melbourne")]
    [InlineData("AU", null)]
    [InlineData("", "Melbourne")]
    [InlineData("AU", "")]
    public async Task ReturnsBadRequest_WhenCountryOrCityMissing(string? country, string? city)
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var function = CreateFunction();
        var context = CreateHttpContext(country, city);

        var result = await function.Run(context.Request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ReturnsOk_WhenWeatherFoundInCache()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _weatherCachingService.Setup(x => x.CheckCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new WeatherApiResult("Sunny"));

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        Assert.IsType<OkObjectResult>(result);
    }
    
    
    [Fact]
    public async Task ReturnsOk_WhenWeatherApiReturnsData()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _weatherCachingService.Setup(x => x.CheckCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((WeatherApiResult?)null);
        _weatherApiService.Setup(x => x.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherApiResult("sunny"));

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ReturnsNoContent_WhenWeatherApiReturnsNoDescription()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _weatherCachingService.Setup(x => x.CheckCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((WeatherApiResult?)null);
        _weatherApiService.Setup(x => x.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherApiResult(""));

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ReturnsNotFound_WhenWeatherApiReturnsNotFound()
    {
        _apiKeyValidationService.Setup(x => x.ValidateApiKey(It.IsAny<HttpRequest>()))
            .Returns("valid");
        _rateLimitingService.Setup(x => x.CheckRateLimitingAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _weatherCachingService.Setup(x => x.CheckCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((WeatherApiResult?)null);
        _weatherApiService.Setup(x => x.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Not found", null, System.Net.HttpStatusCode.NotFound));

        var function = CreateFunction();
        var context = CreateHttpContext();

        var result = await function.Run(context.Request);

        Assert.IsType<NotFoundResult>(result);
    }
}
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using WeatherService.Services.ApiKeyValidation;
using WeatherService.Services.OpenWeatherMap;
using WeatherService.Services.RateLimiting;
using WeatherService.Services.WeatherCachingService;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddHttpClient()
    .Configure<JsonSerializerOptions>(options => { options.PropertyNameCaseInsensitive = true; });

// Add redis cache
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("RedisCache"))
);

// Parse Open Weather Map credentials
builder.Services.Configure<OpenWeatherMapConfiguration>(
    builder.Configuration.GetSection("OpenWeatherMapConfiguration")
);

// Parse rate limiting configuration
builder.Services.Configure<RateLimitingConfiguration>(
    builder.Configuration.GetSection("RateLimitingConfiguration")
);

builder.Services
    .AddSingleton<IRateLimitingService, RateLimitingService>()
    .AddSingleton<IWeatherApiService, WeatherApiService>()
    .AddSingleton<IWeatherCachingService, WeatherCachingService>();

// In production, these wouldn't be validated in application code
HashSet<string> validApiKeys = new HashSet<string>
{
    "e1a7c3b9d5f2a8e4c6b1d9f3a2e7c4b8",
    "f4d2b6e8a1c9f7b3e5a2d8c6b0f1e3a7",
    "c8f1a3d7e9b2c4a6f5d3b8e2a1c7f9d4",
    "a9e3c7b1f6d2a8c5e4b0d7f3a2c6e8b5",
    "d6b2e8c4a1f9b7d3e5c0a8f2b4d1c7e9"
};

builder.Services.AddSingleton<IApiKeyValidationService, ApiKeyValidationService>(
    sp => new ApiKeyValidationService(validApiKeys));

builder.Build().Run();
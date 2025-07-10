using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using WeatherService.Services.OpenWeatherMap;
using WeatherService.Services.RateLimiting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddHttpClient()
    .Configure<JsonSerializerOptions>(options =>
    {
        options.PropertyNameCaseInsensitive = true;
    });


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
    .AddSingleton<IWeatherApiService, WeatherApiService>();

builder.Build().Run();
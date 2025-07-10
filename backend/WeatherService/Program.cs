using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherService.Services.OpenWeatherMap;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddHttpClient()
    .Configure<JsonSerializerOptions>(options =>
    {
        options.PropertyNameCaseInsensitive = true;
    });


// Add redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("RedisCache");
    options.InstanceName = "MyRedisInstance";
});

// Parse Open Weather Map credentials
builder.Services.Configure<OpenWeatherMapConfiguration>(
    builder.Configuration.GetSection("OpenWeatherMapConfiguration")
);

builder.Services
    .AddSingleton<IWeatherApiService, WeatherApiService>();

builder.Build().Run();
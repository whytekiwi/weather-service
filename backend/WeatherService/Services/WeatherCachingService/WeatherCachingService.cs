using System.Text.Json;
using StackExchange.Redis;
using WeatherService.Models;

namespace WeatherService.Services.WeatherCachingService;

/// <inheritdoc />
public class WeatherCachingService(IConnectionMultiplexer redis) : IWeatherCachingService
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<WeatherApiResult?> CheckCacheAsync(string city, string country)
    {
        var key = FormatCacheKey(city, country);
        var result = await _redisDb.StringGetAsync(key);
        return result.HasValue
            ? JsonSerializer.Deserialize<WeatherApiResult>(result.ToString())
            : null;
    }

    /// <inheritdoc />
    public async Task AddToCacheAsync(string city, string country, WeatherApiResult weather)
    {
        var key = FormatCacheKey(city, country);
        var json = JsonSerializer.Serialize(weather);
        await _redisDb.StringSetAsync(key, json, _cacheDuration);
    }

    private static string FormatCacheKey(string city, string country)
    {
        // Prefix with W (for weather) to prevent key collisions
        return $"W:{city},{country}";
    }
}
using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.Options;
using WeatherService.Models;

namespace WeatherService.Services.OpenWeatherMap;

/// <inheritdoc />
public class WeatherApiService : IWeatherApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _url;
    private readonly string _apiKey;

    public WeatherApiService(IOptions<OpenWeatherMapConfiguration> configuration, IHttpClientFactory httpClientFactory)
    {
        // Create readable exceptions if expected values are null
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration?.Value?.ApiKey, "OpenWeatherMapConfiguration:ApiKey");
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration?.Value?.Url, "OpenWeatherMapConfiguration:Url");

        _httpClientFactory = httpClientFactory;
        _url = configuration.Value.Url;
        _apiKey = configuration.Value.ApiKey;
    }

    /// <inheritdoc />
    public async Task<WeatherApiResult?> GetWeatherAsync(string city, string country,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();

        var builder = new UriBuilder(_url);
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

        query["q"] = $"{city},{country}";
        query["appid"] = _apiKey;

        builder.Query = query.ToString();
        var url = builder.ToString();

        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        // Requirements are to simply return the "description".
        // For memory efficiency, you could use a stream reader to only fetch the required string
        // I opted to parse the full document, allowing for the feature set of this API to be extended in a much simpler way
        var result = await response.Content.ReadFromJsonAsync<ApiResult>(cancellationToken);

        return result?.MapToApiResult();
    }
}
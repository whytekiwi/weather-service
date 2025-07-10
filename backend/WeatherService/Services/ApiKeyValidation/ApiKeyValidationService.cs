using Microsoft.AspNetCore.Http;

namespace WeatherService.Services.ApiKeyValidation;

/// <inheritDoc />
public class ApiKeyValidationService(HashSet<string> validApiKeys) : IApiKeyValidationService
{
    private const string ApiKeyHeader = "X-API-KEY";
    private const string ApiKeyQueryParam = "api_key";

    /// <inheritDoc />
    public string ValidateApiKey(HttpRequest req)
    {
        // Check if the API key is present in the headers or query parameters   
        string? apiKey = req.Headers[ApiKeyHeader].FirstOrDefault() ?? req.Query[ApiKeyQueryParam].FirstOrDefault();

        if (string.IsNullOrEmpty(apiKey))
            throw new ApiKeyValidationException();

        if (!validApiKeys.Contains(apiKey))
            throw new ApiKeyValidationException();

        return apiKey;
    }
}
using Microsoft.AspNetCore.Http;

namespace WeatherService.Services.ApiKeyValidation;

/// <inheritDoc />
public class ApiKeyValidationService(HashSet<string> validApiKeys) : IApiKeyValidationService
{
    private const string API_KEY_HEADER = "X-API-KEY";
    private const string API_KEY_QUERY_PARAM = "api_key";

    /// <inheritDoc />
    public string ValidateApiKey(HttpRequest req)
    {
        // Check if the API key is present in the headers or query parameters   
        string? apiKey = req.Headers[API_KEY_HEADER].FirstOrDefault() ?? req.Query[API_KEY_QUERY_PARAM].FirstOrDefault();

        if (string.IsNullOrEmpty(apiKey))
            throw new ApiKeyValidationException();

        if (!validApiKeys.Contains(apiKey))
            throw new ApiKeyValidationException();

        return apiKey;
    }
}
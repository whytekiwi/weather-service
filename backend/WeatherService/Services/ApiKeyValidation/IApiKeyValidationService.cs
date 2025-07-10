
using Microsoft.AspNetCore.Http;

namespace WeatherService.Services.ApiKeyValidation;


/// <summary>
/// Service for fetching and validating API keys.
/// </summary>
public interface IApiKeyValidationService
{
    /// <summary>
    /// Validates the API key present in the request.
    /// </summary>
    /// <param name="req">The incoming HTTP request.</param>
    /// <returns>The user ID from the API Key</returns>
    /// <exception cref="ApiKeyValidationException">If the API Key is not set, or invalid.</exception>
    /// <remarks>Usually we would resolve a user id from the api key, here we are just using the same value.</remarks>
    string ValidateApiKey(HttpRequest req);
}
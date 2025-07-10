using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using WeatherService.Services.ApiKeyValidation;

namespace WeatherService.Tests;

public class ApiKeyValidationServiceTest
{
    private readonly HashSet<string> validApiKeys = new HashSet<string>
    {
        "e1a7c3b9d5f2a8e4c6b1d9f3a2e7c4b8",
        "f4d2b6e8a1c9f7b3e5a2d8c6b0f1e3a7",
        "c8f1a3d7e9b2c4a6f5d3b8e2a1c7f9d4",
        "a9e3c7b1f6d2a8c5e4b0d7f3a2c6e8b5",
        "d6b2e8c4a1f9b7d3e5c0a8f2b4d1c7e9"
    };

    [Fact]
    public void ValiateApiKey_WithValidApiKeyInHeader_ReturnsUserId()
    {
        // Arrange
        var httpReq = new Mock<HttpRequest>();
        string validApiKey = validApiKeys.First();
        var service = new ApiKeyValidationService(validApiKeys);

        HeaderDictionary headers = new HeaderDictionary
        {
            { "X-API-KEY", validApiKey }
        };

        httpReq.Setup(req => req.Headers).Returns(headers);
        httpReq.Setup(req => req.Query).Returns(new QueryCollection());

        // Act 
        string userId = service.ValidateApiKey(httpReq.Object);

        // Assert
        Assert.Equal(validApiKey, userId);
    }

    [Fact]
    public void ValiateApiKey_WithValidApiKeyInQuery_ReturnsUserId()
    {
        // Arrange
        var httpReq = new Mock<HttpRequest>();
        string validApiKey = validApiKeys.First();
        var service = new ApiKeyValidationService(validApiKeys);

        httpReq.Setup(req => req.Headers).Returns(new HeaderDictionary());
        httpReq.Setup(req => req.Query).Returns(new QueryCollection(new Dictionary<string, StringValues>
        {
            { "api_key", validApiKey }
        }));

        // Act 
        string userId = service.ValidateApiKey(httpReq.Object);

        // Assert
        Assert.Equal(validApiKey, userId);
    }

    [Fact]
    public void ValiateApiKey_WithNoApiKey_ThrowsException()
    {
        // Arrange
        var httpReq = new Mock<HttpRequest>();
        httpReq.Setup(req => req.Headers).Returns(new HeaderDictionary());
        httpReq.Setup(req => req.Query).Returns(new QueryCollection());
        var service = new ApiKeyValidationService(validApiKeys);

        // Act & Assert
        Assert.Throws<ApiKeyValidationException>(() =>
        {
            string userId = service.ValidateApiKey(httpReq.Object);
        });
    }

    [Fact]
    public void ValiateApiKey_WithInvalidApiKeyInHeader_ThrowsException()
    {
        // Arrange
        var httpReq = new Mock<HttpRequest>();
        var service = new ApiKeyValidationService(validApiKeys);

        HeaderDictionary headers = new HeaderDictionary
        {
            { "X-API-KEY", "invalid_api_key" }
        };

        httpReq.Setup(req => req.Headers).Returns(headers);
        httpReq.Setup(req => req.Query).Returns(new QueryCollection());

        // Act + Assert
        Assert.Throws<ApiKeyValidationException>(() =>
        {
            string userId = service.ValidateApiKey(httpReq.Object);
        });
    }
}
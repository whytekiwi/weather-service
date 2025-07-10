using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using WeatherService.Services.OpenWeatherMap;

namespace WeatherService.Tests
{
    public class WeatherApiServiceTest
    {
        private static WeatherApiService CreateService(HttpResponseMessage responseMessage)
        {
            var config = Options.Create(new OpenWeatherMapConfiguration
            {
                ApiKey = "test-api-key",
                Url = "https://api.openweathermap.org/data/2.5/weather"
            });

            // I understand contention around the Moq library, however I wanted quick wins
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return new WeatherApiService(config, httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task GetWeather_ReturnsWeatherApiResult_WhenApiResponseIsValid()
        {
            // Arrange
            var apiResponse = new 
            {
                Weather = new[] { new Weather("clear sky") }
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(apiResponse)
            };
            var service = CreateService(response);

            // Act
            var result = await service.GetWeather("Melbourne", "AU");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("clear sky", result.Description);
        }
        
        [Fact]
        public async Task GetWeather_ReturnsNull_WhenApiResponseIsEmpty()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new object())
            };
            var service = CreateService(response);

            // Act
            var result = await service.GetWeather("Melbourne", "AU");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeather_ThrowsException_WhenResponseIsNotSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var service = CreateService(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeather("Melbourne", "AU"));
        }
    }
}
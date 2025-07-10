using WeatherService.Services.OpenWeatherMap;

namespace WeatherService.Tests;

public class WeatherApiModelMapperTest
{
    [Fact]
    public void MapToApiResult_ReturnsNull_WhenApiResultIsNull()
    {
        ApiResult? apiResult = null;
        var result = apiResult.MapToApiResult();
        Assert.Null(result);
    }

    [Fact]
    public void MapToApiResult_ReturnsNull_WhenWeatherIsNull()
    {
        var apiResult = new ApiResult(null);
        var result = apiResult.MapToApiResult();
        Assert.Null(result);
    }

    [Fact]
    public void MapToApiResult_ReturnsNull_WhenWeatherIsEmpty()
    {
        var apiResult = new ApiResult(Array.Empty<Weather>());
        var result = apiResult.MapToApiResult();
        Assert.Null(result);
    }

    [Fact]
    public void MapToApiResult_ReturnsWeatherApiResult_WithDescription()
    {
        var expectedDescription = "clear sky";
        var apiResult = new ApiResult(new[] { new Weather(expectedDescription) });
        var result = apiResult.MapToApiResult();
        Assert.NotNull(result);
        Assert.Equal(expectedDescription, result!.Description);
    }

    [Fact]
    public void MapToApiResult_ReturnsWeatherApiResult_WithNullDescription()
    {
        var apiResult = new ApiResult(new[] { new Weather(null) });
        var result = apiResult.MapToApiResult();
        Assert.NotNull(result);
        Assert.Null(result!.Description);
    }
}
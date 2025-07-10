# Weather Service

This is the backend component for the challenge. This API contains a single endpoint, which accepts two query parameters:
- `city`: The name of the city to request weather for
- `country`: The name of the country to request weather for

TODO: rate limiting + api key documentation

It forwards the request onto the OpenWeatherApi service, and if successful returns the description for the weather in the requested city.

## Dependencies

- Redis: https://redis.io/docs/latest/operate/oss_and_stack/install/archive/install-redis/
- Dotnet 9.0: https://dotnet.microsoft.com/en-us/download
- Azure functions runtime: https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local

## Set Up

You'll need to set some environment variables:
- `RedisCache`: The connection string for your running redis instance.
- `OpenWeatherMapConfiguration:Url`: The url for the OpenWeatherApi service
- `OpenWeatherMapConfiguration:ApiKey`: The api key for the OpenWeatherApi service

This is best achieved with a `local.settings.json` file stored in the [Weather Service](/WeatherService/) directory. This file is ommited for security purposes. An example file:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "RedisCache": "localhost",
    "OpenWeatherMapConfiguration:Url": "https://api.openweathermap.org/data/2.5/weather",
    "OpenWeatherMapConfiguration:ApiKey": ""
  }
}
```


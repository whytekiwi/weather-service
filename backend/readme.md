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
- `OpenWeatherMapConfiguration:Url`: The url for the OpenWeatherApi service.
- `OpenWeatherMapConfiguration:ApiKey`: The api key for the OpenWeatherApi service.

There are also 2 optional environment variables:
- `RateLimitingConfiguration:TimeoutMinutes`: How long the timeout period lasts for. Defaults to 60.
- `RateLimitingConfiguration:RetryLimit`:  How many requests a single user could make in the specified time period. Defaults to 5.

This is best achieved with a `local.settings.json` file stored in the [Weather Service](/WeatherService/) directory. This file is ommited for security purposes. An example file:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "RedisCache": "localhost",
    "OpenWeatherMapConfiguration:Url": "https://api.openweathermap.org/data/2.5/weather",
    "OpenWeatherMapConfiguration:ApiKey": "",
    "RateLimitingConfiguration:TimeoutMinutes": "5",
    "RateLimitingConfiguration:RetryLimit": "10"
  }
}
```

## Considerations

1. Sliding expiration for rate limiting

I have not implemented a "sliding scale" rate limiter for this application. In production, I'd prefer to not do rate limiting in the application code, instead relying on tools like APIM. This would prevent invocations of our function app when we expect it to fail (very usefull under high load applications).

A sliding scale would mean that a user can make a 6th request exactly after the hour has passed since first invocation, but not a 7th if an hour has not passed since thier second invocation.

My implementaion gives the full alloaction of requests back after the first request has timed out, which does mean it's possible for a user to make more than 5 requests per hour:
- Make a request (start 60 minute timeout)
- Wait 50 minutes
- Make anywhere between 1-4 requests, eventually get rate limited
- Wait 10 minutes, full allocation of 5 requests is reset
- Make anywhere between 1-5 requests
- This means that in the last 10 minutes, you may have made more than 5 requests

There are design patterns to make this work, but with Redis, setting a sliding expiration on the keys would result in the user needing to wait the full hour after making their last request, which isn't ideal behaviour.

I could talk hours about time, and how it's all made up (when is an hour not an hour). In this instance, I think this is a pretty standard implementation of rate limiting, but I wanted to add it here as a consideration.

2. Optismitic Rate Limiting

In the current implmentation, we decrease the remainng retries for the rate limit before we even know if the request is successful or not. This means that if we have an error on our end, the user is still "charged" for the attempt.

Ideally we wouldn't decrease th4e count until we know it's successful, however that opens up the concurrency edge case (same user making multiple requests in parallel), so I opted for optimistic rate limiting.


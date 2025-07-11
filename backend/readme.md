# Weather Service

This is the backend component for the challenge. This API contains a single endpoint, which accepts two query parameters:
- `city`: The name of the city to request weather for
- `country`: The name of the country to request weather for

It forwards the request on to the OpenWeatherApi service, and if successful returns the description for the weather in the requested city.

## API usage

```
NOTE: I'd usually do this with swagger, generated at build time using CI pipelines and hosted alongside the endpoints. That felt a little outside the scope of this project.

Alternatives exist, include docs generated at runtime, which does come with performance impacts, but less development overhead.
```

When running, this service will open 1 HTTP endpoint ready to be consumed (using tools like CURL, Postman, etc).

- http://localhost:7071/api/FetchWeather: Query the current weather in a `city` and `country`.

This request expects 2 query parameters:
- `city`: The name of the city to query the weather for
- `country`: The name of the country your city is in

To authenticate this request, you must provide an API key. You can do this in two ways:
- Add an HTTP header with the key `x-api-key` and value being the key
- Add a query parameter with the name `api_key` and the value being the selected key

A set of 5 dev api keys have been generated, and can each be used up to 5 times an hour (can be changed with application config)

```
e1a7c3b9d5f2a8e4c6b1d9f3a2e7c4b8
f4d2b6e8a1c9f7b3e5a2d8c6b0f1e3a7
c8f1a3d7e9b2c4a6f5d3b8e2a1c7f9d4
a9e3c7b1f6d2a8c5e4b0d7f3a2c6e8b5
d6b2e8c4a1f9b7d3e5c0a8f2b4d1c7e9
```



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

This is best achieved with a `local.settings.json` file stored in the [Weather Service](/WeatherService/) directory. This file is omitted for security purposes. An example file:

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

## Build

To build the project, run the following command from the root directory:

```sh
dotnet build
```

## Run

To run the Azure Functions project locally:

1. Ensure you have set up your `local.settings.json` as described above.
2. Start the function app:

```sh
cd WeatherService
func start
```

Or, using the .NET CLI:

```sh
dotnet run --project WeatherService
```

## Test

To run the unit tests:

```sh
dotnet test
```

Or, to run tests for the specific test project:

```sh
dotnet test WeatherService.Tests
```

---

Make sure you have all dependencies installed (see [Dependencies](#dependencies) above) before running or testing the project.

## Considerations

### 1. Sliding expiration for rate limiting

I have not implemented a "sliding scale" rate limiter for this application. In production, I'd prefer to not do rate limiting in the application code, instead relying on tools like APIM. This would prevent invocations of our function app when we expect it to fail (very useful under high load applications).

A sliding scale would mean that a user can make a 6th request exactly after the hour has passed since first invocation, but not a 7th if an hour has not passed since their second invocation.

My implementation gives the full allocation of requests back after the first request has timed out, which does mean it's possible for a user to make more than 5 requests per hour:
- Make a request (start 60 minute timeout)
- Wait 50 minutes
- Make anywhere between 1-4 requests, eventually get rate limited
- Wait 10 minutes, full allocation of 5 requests is reset
- Make anywhere between 1-5 requests
- This means that in the last 10 minutes, you may have made more than 5 requests

There are design patterns to make this work, but with Redis, setting a sliding expiration on the keys would result in the user needing to wait the full hour after making their last request, which isn't ideal behavior.

I could talk hours about time, and how it's all made up (when is an hour not an hour). In this instance, I think this is a pretty standard implementation of rate limiting, but I wanted to add it here as a consideration.

### 2. Optimistic Rate Limiting

In the current implementation, we decrease the remaining retries for the rate limit before we even know if the request is successful or not. This means that if we have an error on our end, the user is still "charged" for the attempt.

Ideally we wouldn't decrease th4e count until we know it's successful, however that opens up the concurrency edge case (same user making multiple requests in parallel), so I opted for optimistic rate limiting.

### 3. API Key validation

API Key validation (and in general the whole authorization + authentication flow) is best handled in front of microservices. The code should focus on the business needs, while allowing other services like APIM to handle the intricacies required.

In general, it should be impossible for a user to invocate a call through to our functions if they aren't authenticated, saving expensive compute time.

I'd also never store secrets in plain text in a repo either.


# RESTSchemaRetry

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/RESTSchemaRetry?style=plastic)](https://www.nuget.org/packages/RESTSchemaRetry)
[![issues - RESTSchemaRetry](https://img.shields.io/github/issues/engineering87/RESTSchemaRetry)](https://github.com/engineering87/RESTSchemaRetry/issues)
[![stars - RESTSchemaRetry](https://img.shields.io/github/stars/engineering87/RESTSchemaRetry?style=social)](https://github.com/engineering87/RESTSchemaRetry)

RESTSchemaRetry is a .NET library that implements a simple **Schema-Retry** pattern in REST API context to enhance application resilience.
A Schema-Retry handles transient failures when attempting to connect to a service or network resource by transparently retrying failed operations.

### How it works
RESTSchemaRetry implements a straightforward retry mechanism with a delay, managing communication timeouts or service errors transparently to the application.
The retry mechanism checks the HTTP response code to determine whether the error is transient or not.

### Transient error recovery
Below is the list of potentially transient errors handled by RESTSchemaRetry:
  * TooManyRequests
  * InternalServerError
  * BadGateway
  * ServiceUnavailable
  * GatewayTimeout
  * InsufficientStorage
  * RequestTimeout
  * HttpVersionNotSupported
  * NetworkAuthenticationRequired

### Supported Backoff Types

- **Constant Backoff**: The delay between retries is constant and does not change. This is a simple approach where each retry attempt waits for a fixed amount of time, defined by a `RetryDelay` parameter.
  
- **Linear Backoff**: The delay between retries increases linearly. Each retry attempt waits longer than the previous one by a fixed increment. The delay is calculated as `RetryDelay * (retry + 1)`.

- **Exponential Backoff**: The delay between retries grows exponentially. The delay is calculated using a power of 2, multiplied by the base RetryDelay. This approach is useful to quickly back off from a failing operation, reducing server load. Formula: `RetryDelay * (2 ^ retry)`.
  
- **Exponential Backoff with Jitter**: Similar to exponential backoff, but with added randomness ("jitter") to the delay to avoid synchronized retries from multiple clients, which can cause spikes in traffic. The delay is calculated as `RetryDelay * (2 ^ retry) * random_factor`, where `random_factor` is a random value between 0 and 1.
  
- **Fibonacci Backoff**: The delay between retries follows the Fibonacci sequence. This provides a middle ground between linear and exponential backoff, growing less aggressively than exponential. The delay is calculated as `RetryDelay * Fibonacci(retry)`.
  
- ***Randomized Backoff**: The delay is randomly chosen between a minimum and a maximum range, usually defined as a multiple of RetryDelay. This helps to distribute retries more evenly over time, avoiding bursts of traffic. The delay is calculated randomly within a range, e.g., `[RetryDelay, RetryDelay * 2]`.

### How to use it

To use the RESTSchemaRetry library, just instantiate a **RetryClient** object specifying the base URL and the resource.
There are multiple constructor to specify the REST API parameters.

```csharp
var retryClient = new RetryClient("https://example.com/","resource");
```
at this point the REST API requests can be performed.
For example, below how to execute a POST request:

```csharp
var response = retryClient.Post<T>(objectToPost);
```

RESTSchemaRetry uses the **RestSharp** library to execute the web requests.

### NuGet Package

The library is available on NuGet packetmanager.
https://www.nuget.org/packages/RESTSchemaRetry/

### RestSharp Reference
RESTSchemaRetry uses the **RestSharp** library, which is distributed under Apache 2.0 license.
* [Official Project](https://github.com/restsharp/RestSharp)
* [License](https://github.com/restsharp/RestSharp/blob/dev/LICENSE.txt)

## Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/RESTSchemaRetry/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
RESTSchemaRetry source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre[at]protonmail.com for any details.

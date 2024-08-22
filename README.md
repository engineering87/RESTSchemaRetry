# RESTSchemaRetry

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/RESTSchemaRetry?style=plastic)](https://www.nuget.org/packages/RESTSchemaRetry)
[![issues - RESTSchemaRetry](https://img.shields.io/github/issues/engineering87/RESTSchemaRetry)](https://github.com/engineering87/RESTSchemaRetry/issues)
[![stars - RESTSchemaRetry](https://img.shields.io/github/stars/engineering87/RESTSchemaRetry?style=social)](https://github.com/engineering87/RESTSchemaRetry)

RESTSchemaRetry is a .NET library that implements a simple Schema-Retry pattern in the context of REST services to enhance application stability.
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
Please contact at francesco.delre.87[at]protonmail.com for any details.

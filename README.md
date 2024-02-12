[![Github license](mit.svg)](https://github.com/engineering87/RESTSchemaRetry/blob/master/LICENSE)

# RESTSchemaRetry
RESTSchemaRetry is a C# library that implements a simple Schema-Retry pattern in REST services context to improve the stability of the application.
A Schema-Retry can handle transient failures when it tries to connect to a service or network resource, by transparently retrying a failed operation. 

### How it works
RESTSchemaRetry implements a simple after delay retry schema, managing communication timeouts or service error transparently to the application.
The retry schema check the HTTP response code to decide if the error is transient or not.

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

RESTSchemaRetry uses the RestSharp library to execute the web requests.

### NuGet Package

The library is available on NuGet packetmanager.
https://www.nuget.org/packages/RESTSchemaRetry/

### RestSharp Reference
RESTSchemaRetry uses the **RestSharp** library, which is distributed under Apache 2.0 license.
* [Official Project](https://github.com/restsharp/RestSharp)
* [License](https://github.com/restsharp/RestSharp/blob/dev/LICENSE.txt)

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/RESTSchemaRetry/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
RESTSchemaRetry source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.

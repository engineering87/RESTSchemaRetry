// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Interfaces;
using RESTSchemaRetry;
using RESTSchemaRetry.TestClient.Model;
using RESTSchemaRetry.Enum;

Console.WriteLine("### POST ###");
PostExample();

Console.WriteLine("\n### GET ###");
GetExample();

Console.WriteLine("\n### PUT ###");
PutExample();

Console.WriteLine("\n### DELETE ###");
DeleteExample();

Console.WriteLine("\n### PATCH ###");
PatchExample();

Console.WriteLine("\n### OPTIONS ###");
OptionsExample();

Console.WriteLine("\n### ERROR ###");
SimulateError();

Console.WriteLine("\n### TRANSIENT ERROR ###");
await SimulateTransientError();

/// <summary>
/// Sends a POST request to create a new resource.
/// </summary>
static void PostExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts");

    var response = client.Post<object, PostResponse>(
        new { title = "foo", body = "bar", userId = 1 }
    );

    PrintResponse(response);
}

/// <summary>
/// Sends a GET request to retrieve a specific resource.
/// </summary>
static void GetExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts/1");

    var response = client.Get<PostResponse>();
    PrintResponse(response);
}

/// <summary>
/// Sends a PUT request to update an existing resource.
/// </summary>
static void PutExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts/1");

    var updatedPost = new { id = 1, title = "updated title", body = "updated body", userId = 1 };
    var response = client.Put<object, PostResponse>(updatedPost);

    PrintResponse(response);
}

/// <summary>
/// Sends a DELETE request to remove a specific resource.
/// </summary>
static void DeleteExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts/1");

    var response = client.Delete<object, object>(new { });
    PrintResponse(response);
}

/// <summary>
/// Sends a PATCH request to partially update a specific resource.
/// </summary>
static void PatchExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts/1");

    var patchData = new { title = "patched title" };
    var response = client.Patch<object, PostResponse>(patchData);

    PrintResponse(response);
}

/// <summary>
/// Sends an OPTIONS request to retrieve the communication options available.
/// </summary>
static void OptionsExample()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "posts");

    var response = client.Options<object>();
    PrintResponse(response);
}

/// <summary>
/// Simulates a failed GET request to test error handling.
/// </summary>
static void SimulateError()
{
    IRetryClient client = new RetryClient("https://jsonplaceholder.typicode.com", "invalid-resource");

    var response = client.Get<PostResponse>();
    PrintResponse(response);
}

/// <summary>
/// Simulates a transient error (e.g. HTTP 500) and tests the retry mechanism.
/// </summary>
static async Task SimulateTransientError()
{
    // httpbin.org/status/500
    string baseUrl = "https://httpbin.org";
    string resource = "status/500";

    IRetryClient client = new RetryClient(baseUrl, resource)
    {
        RetryNumber = 3,
        RetryDelay = TimeSpan.FromSeconds(2),
        DelayType = BackoffTypes.ExponentialFullJitter
    };

    var response = await client.GetAsync<object>();

    Console.WriteLine($"Status Code: {response.StatusCode}");
    Console.WriteLine($"Is Successful: {response.IsSuccessful}");
    Console.WriteLine($"Content: {response.Content}");

    if (!response.IsSuccessful)
    {
        Console.WriteLine($"Error Message: {response.ErrorMessage}");
    }
}

/// <summary>
/// Prints details of the REST response including status, success flag, and content.
/// </summary>
/// <typeparam name="T">Type of the response content.</typeparam>
/// <param name="response">The REST response to print.</param>
static void PrintResponse<T>(RestSharp.RestResponse<T> response)
{
    Console.WriteLine($"Status Code: {response.StatusCode}");
    Console.WriteLine($"Is Successful: {response.IsSuccessful}");
    Console.WriteLine($"Content: {response.Content}");

    if (!response.IsSuccessful)
    {
        Console.WriteLine($"Error Message: {response.ErrorMessage}");
    }
}
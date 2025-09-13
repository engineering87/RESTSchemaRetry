// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Interfaces;
using RESTSchemaRetry.TestClient.Model;
using RESTSchemaRetry.Enum;
using RestSharp;
using System.Diagnostics;

namespace RESTSchemaRetry.TestClient
{
    internal class Program
    {
        /// <summary>
        /// Entry point. Runs sync/async tests and prints a final aggregated summary.
        /// </summary>
        public static async Task Main()
        {
            var results = new List<TestResult>();

            Console.WriteLine("### POST (sync) ###");
            PostExample(results);

            Console.WriteLine("\n### GET (sync) ###");
            GetExample(results);

            Console.WriteLine("\n### PUT (sync) ###");
            PutExample(results);

            Console.WriteLine("\n### DELETE (sync) ###");
            DeleteExample(results);

            Console.WriteLine("\n### PATCH (sync) ###");
            PatchExample(results);

            Console.WriteLine("\n### OPTIONS (sync) ###");
            OptionsExample(results);

            Console.WriteLine("\n### ERROR (sync) ###");
            SimulateError(results);

            // --- Async tests ---
            Console.WriteLine("\n### GET with multiple query parameters (async) ###");
            await GetWithQueryParamsAsync(results);

            Console.WriteLine("\n### HEAD (sync) ###");
            HeadExample(results);

            Console.WriteLine("\n### HEAD (async) ###");
            await HeadAsyncExample(results);

            Console.WriteLine("\n### GET (async) with resource normalization (no leading slash) ###");
            await GetAsync_NoLeadingSlashResource(results);

            Console.WriteLine("\n### OPTIONS (async) ###");
            await OptionsAsyncExample(results);

            Console.WriteLine("\n### TRANSIENT ERROR (500) with retries (async) ###");
            await SimulateTransientError(results);

            // --- Final stats ---
            PrintStatistics(results);
        }

        // ======================================================================
        // Types & Helpers
        // ======================================================================

        /// <summary>
        /// Holds the outcome of a single test case.
        /// </summary>
        public record TestResult(
            string Name,
            string HttpMethod,
            Uri BaseUri,
            string Resource,
            int StatusCode,
            bool IsSuccessful,
            long DurationMs,
            string ErrorMessage
        );

        /// <summary>
        /// Prints details of the REST response including status, success flag, and content.
        /// </summary>
        /// <typeparam name="T">Type of the response content.</typeparam>
        /// <param name="response">The REST response to print.</param>
        private static void PrintResponse<T>(RestResponse<T> response)
        {
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Is Successful: {response.IsSuccessful}");
            Console.WriteLine($"Content: {response.Content}");

            if (!response.IsSuccessful)
                Console.WriteLine($"Error Message: {response.ErrorMessage}");
        }

        /// <summary>
        /// Prints response headers if present.
        /// </summary>
        /// <param name="response">The REST response (untyped) to inspect.</param>
        private static void PrintHeaders(RestResponse response)
        {
            if (response?.Headers == null || response.Headers.Count == 0)
            {
                Console.WriteLine("Headers: (none)");
                return;
            }

            Console.WriteLine("Headers:");
            foreach (var h in response.Headers)
                Console.WriteLine($"  {h.Name}: {h.Value}");
        }

        /// <summary>
        /// Adds a standardized TestResult to the shared results list.
        /// </summary>
        /// <typeparam name="T">Type of the response payload.</typeparam>
        /// <param name="results">The shared results collection.</param>
        /// <param name="name">Human-friendly test name.</param>
        /// <param name="method">HTTP verb used.</param>
        /// <param name="baseUrl">Base URL string.</param>
        /// <param name="resource">Resource path.</param>
        /// <param name="sw">Stopwatch with elapsed time.</param>
        /// <param name="resp">RestSharp typed response.</param>
        private static void Record<T>(
            List<TestResult> results,
            string name,
            string method,
            string baseUrl,
            string resource,
            Stopwatch sw,
            RestResponse<T> resp)
        {
            _ = Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri);

            results.Add(new TestResult(
                Name: name,
                HttpMethod: method,
                BaseUri: baseUri,
                Resource: resource,
                StatusCode: (int)resp.StatusCode,
                IsSuccessful: resp.IsSuccessful,
                DurationMs: sw.ElapsedMilliseconds,
                ErrorMessage: resp.IsSuccessful ? null : resp.ErrorMessage
            ));
        }

        /// <summary>
        /// Prints an aggregated summary of all test results including success ratios and timing.
        /// </summary>
        /// <param name="results">The collected test results.</param>
        private static void PrintStatistics(List<TestResult> results)
        {
            Console.WriteLine("\n==================== TEST SUMMARY ====================");

            var total = results.Count;
            var ok = results.Count(r => r.IsSuccessful);
            var ko = total - ok;
            var successRate = total > 0 ? (double)ok / total * 100.0 : 0.0;
            var avgMs = results.Count > 0 ? results.Average(r => r.DurationMs) : 0.0;

            Console.WriteLine($"Total tests      : {total}");
            Console.WriteLine($"Successful       : {ok}");
            Console.WriteLine($"Failed           : {ko}");
            Console.WriteLine($"Success rate     : {successRate:F1}%");
            Console.WriteLine($"Avg latency (ms) : {avgMs:F1}");

            // By HTTP verb
            var byVerb = results
                .GroupBy(r => r.HttpMethod)
                .Select(g => new
                {
                    Verb = g.Key,
                    Count = g.Count(),
                    Ok = g.Count(x => x.IsSuccessful),
                    Ko = g.Count(x => !x.IsSuccessful),
                    Avg = g.Average(x => x.DurationMs)
                })
                .OrderBy(x => x.Verb);

            Console.WriteLine("\n-- By HTTP Method --");
            foreach (var v in byVerb)
                Console.WriteLine($"{v.Verb,-7} -> total: {v.Count,2} | ok: {v.Ok,2} | ko: {v.Ko,2} | avg(ms): {v.Avg:F1}");

            // Failures details
            if (ko > 0)
            {
                Console.WriteLine("\n-- Failures --");
                foreach (var f in results.Where(r => !r.IsSuccessful))
                    Console.WriteLine($"[{f.HttpMethod}] {f.Name} -> {f.StatusCode} {f.BaseUri} {f.Resource} | {f.ErrorMessage}");
            }

            Console.WriteLine("======================================================\n");
        }

        // ======================================================================
        // TEST CASES (sync/async)
        // ======================================================================

        /// <summary>
        /// Sends a POST request to create a new resource (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void PostExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Post<object, PostResponse>(
                new { title = "foo", body = "bar", userId = 1 });
            sw.Stop();

            PrintResponse(response);
            Record(results, "POST create post", "POST", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends a GET request to retrieve a specific resource (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void GetExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts/1";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Get<PostResponse>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "GET post #1", "GET", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends a PUT request to update an existing resource (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void PutExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts/1";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var updatedPost = new { id = 1, title = "updated title", body = "updated body", userId = 1 };
            var sw = Stopwatch.StartNew();
            var response = client.Put<object, PostResponse>(updatedPost);
            sw.Stop();

            PrintResponse(response);
            Record(results, "PUT post #1", "PUT", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends a DELETE request to remove a specific resource (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void DeleteExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts/1";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Delete<object, object>(new { });
            sw.Stop();

            PrintResponse(response);
            Record(results, "DELETE post #1", "DELETE", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends a PATCH request to partially update a specific resource (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void PatchExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts/1";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var patchData = new { title = "patched title" };
            var sw = Stopwatch.StartNew();
            var response = client.Patch<object, PostResponse>(patchData);
            sw.Stop();

            PrintResponse(response);
            Record(results, "PATCH post #1", "PATCH", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends an OPTIONS request to retrieve the communication options available (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void OptionsExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Options<object>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "OPTIONS posts (sync)", "OPTIONS", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Simulates a failed GET request to test error handling (synchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void SimulateError(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "invalid-resource";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Get<PostResponse>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "GET invalid resource", "GET", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Executes a GET request with multiple query parameters (asynchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static async Task GetWithQueryParamsAsync(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "comments";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var qp = new Dictionary<string, string> { ["postId"] = "1" };

            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync<object>(qp);
            sw.Stop();

            PrintResponse(response);
            Record(results, "GET comments?postId=1 (async)", "GET", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Issues a HEAD request (synchronous) and prints headers.
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static void HeadExample(List<TestResult> results)
        {
            const string baseUrl = "https://httpbin.org";
            const string resource = "get";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = client.Head<object>();
            sw.Stop();

            PrintResponse(response);
            PrintHeaders(response);
            Record(results, "HEAD /get (sync)", "HEAD", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Issues a HEAD request (asynchronous) and prints headers.
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static async Task HeadAsyncExample(List<TestResult> results)
        {
            const string baseUrl = "https://httpbin.org";
            const string resource = "get";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = await client.HeadAsync<object>();
            sw.Stop();

            PrintResponse(response);
            PrintHeaders(response);
            Record(results, "HEAD /get (async)", "HEAD", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Executes an async GET on a resource path without a leading slash to ensure normalization works.
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static async Task GetAsync_NoLeadingSlashResource(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts/1"; // no leading slash on purpose
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync<PostResponse>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "GET post #1 (async, normalized)", "GET", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Sends an OPTIONS request (asynchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static async Task OptionsAsyncExample(List<TestResult> results)
        {
            const string baseUrl = "https://jsonplaceholder.typicode.com";
            const string resource = "posts";
            IRetryClient client = new RetryClient(baseUrl, resource);

            var sw = Stopwatch.StartNew();
            var response = await client.OptionsAsync<object>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "OPTIONS posts (async)", "OPTIONS", baseUrl, resource, sw, response);
        }

        /// <summary>
        /// Simulates a transient server error (HTTP 500) to test the retry mechanism (asynchronous).
        /// </summary>
        /// <param name="results">The shared results collection to update.</param>
        private static async Task SimulateTransientError(List<TestResult> results)
        {
            // httpbin.org/status/500
            const string baseUrl = "https://httpbin.org";
            const string resource = "status/500";

            IRetryClient client = new RetryClient(baseUrl, resource)
            {
                RetryNumber = 3,
                RetryDelay = TimeSpan.FromSeconds(1),
                DelayType = BackoffTypes.ExponentialFullJitter
            };

            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync<object>();
            sw.Stop();

            PrintResponse(response);
            Record(results, "GET 500 with retries (async)", "GET", baseUrl, resource, sw, response);
        }
    }
}
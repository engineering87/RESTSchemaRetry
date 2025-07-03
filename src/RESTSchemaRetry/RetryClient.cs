// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Enum;
using RESTSchemaRetry.Interfaces;
using RESTSchemaRetry.Provider;
using RESTSchemaRetry.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry
{
    /// <summary>
    /// REST ApiClient with Schema-Retry implementation
    /// </summary>
    public sealed class RetryClient : IRetryClient
    {
        private readonly RestApi _restApi;
        public int RetryNumber { get; set; }
        public TimeSpan RetryDelay { get; set; }
        public BackoffTypes DelayType { get; set; }

        #region Default

        private const int DefaultRetry = 1;
        private const int DefaultDelay = 5; // sec

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL and resource path.
        /// Sets default retry parameters including the retry count, delay, and delay type.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        public RetryClient(string baseUrl, string resource)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, DefaultDelay);
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path, 
        /// and a custom retry delay in milliseconds. Sets default retry count and delay type.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="retryDelayMs">The delay between retries, in milliseconds.</param>
        public RetryClient(string baseUrl, string resource, int retryDelayMs)
        {
            if (retryDelayMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelayMs), "Retry delay must be non-negative.");
            }

            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path, 
        /// and a custom backoff type for retry delays. Sets default retry count and delay duration.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="backoffTypes">The backoff strategy to use for retry delays (e.g., constant, exponential).</param>
        public RetryClient(string baseUrl, string resource, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, DefaultDelay);
            this.DelayType = backoffTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path,
        /// custom retry count, and delay in milliseconds. Sets a constant delay type for retries.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="retryNumber">The number of retry attempts. If a negative value is provided, it defaults to 1.</param>
        /// <param name="retryDelayMs">The delay between retries, in milliseconds.</param>

        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs)
        {
            if (retryDelayMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelayMs), "Retry delay must be non-negative.");
            }

            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : DefaultRetry;
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path, 
        /// custom retry count, delay in milliseconds, and backoff type for retry logic.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="retryNumber">The number of retry attempts. If a negative value is provided, it defaults to 1.</param>
        /// <param name="retryDelayMs">The delay between retries, in milliseconds.</param>
        /// <param name="backoffTypes">The backoff strategy to use for retry delays (e.g., constant, exponential).</param>

        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs, BackoffTypes backoffTypes)
        {
            if (retryDelayMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelayMs), "Retry delay must be non-negative.");
            }

            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : DefaultRetry;
            this.DelayType = backoffTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path, 
        /// custom retry count, and delay as a <see cref="TimeSpan"/>. Sets a constant delay type for retries.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="retryNumber">The number of retry attempts. If a negative value is provided, it defaults to 1.</param>
        /// <param name="retryDelay">The delay between retries, as a <see cref="TimeSpan"/>.</param>

        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : DefaultRetry;
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryClient"/> class with the specified base URL, resource path, 
        /// custom retry count, delay as a <see cref="TimeSpan"/>, and backoff type for retry logic.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint.</param>
        /// <param name="resource">The specific resource path to access within the API.</param>
        /// <param name="retryNumber">The number of retry attempts. If a negative value is provided, it defaults to 1.</param>
        /// <param name="retryDelay">The delay between retries, as a <see cref="TimeSpan"/>.</param>
        /// <param name="backoffTypes">The backoff strategy to use for retry delays (e.g., constant, exponential).</param>

        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : DefaultRetry;
            this.DelayType = backoffTypes;
        }

        #endregion

        /// <summary>
        /// Executes the specified asynchronous operation with retry logic for transient failures.
        /// Retries the operation according to the configured retry count, delay, and backoff strategy.
        /// </summary>
        /// <typeparam name="T">The return type of the asynchronous operation.</typeparam>
        /// <param name="operation">A function representing the asynchronous operation to execute.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous retry operation, yielding the operation's result.</returns>
        /// <remarks>
        /// The method retries the operation when transient failures occur, as determined by the <see cref="RetryEngine"/>. 
        /// Delays between retries follow the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        private async Task<RestResponse<TResponse>> RetryAsync<TResponse>(Func<Task<RestResponse<TResponse>>> action) where TResponse : new()
        {
            var response = await action();

            if (!RetryEngine.IsTransientStatusCode(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            int retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry >= this.RetryNumber)
                    break;

                await Task.Delay(GetDelay(retry));
                response = await action();
                retry++;
            }
            return response;
        }

        /// <summary>
        /// Executes a given operation and retries it if the response indicates a transient failure.
        /// </summary>
        /// <param name="action">The function to execute, typically a REST operation.</param>
        /// <returns>
        /// The final <see cref="RestResponse"/> after retries, or the initial response if retry is not applicable.
        /// </returns>
        /// <remarks>
        /// Retries are performed only if the response has a transient status code and the configured <c>DelayType</c>
        /// allows retry. The retry attempts will stop either when a successful (Accepted) response is received
        /// or the maximum number of retries is reached.
        /// </remarks>
        private RestResponse<TResponse> Retry<TResponse>(Func<RestResponse<TResponse>> action) where TResponse : new()
        {
            var response = action();

            if (!RetryEngine.IsTransientStatusCode(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            int retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry >= this.RetryNumber)
                    break;

                Task.Delay(GetDelay(retry)).Wait();
                response = action();
                retry++;
            }
            return response;
        }

        /// <summary>
        /// Sends a POST request with the specified object to the configured API endpoint, implementing retry logic for transient failures.
        /// This method is marked as obsolete; it is recommended to use the asynchronous version for better performance and non-blocking behavior.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPost">The object to be serialized and sent in the POST request body.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Post<TRequest, TResponse>(TRequest objectToPost)
            where TRequest : class
            where TResponse : new()
        {
            return Retry(() => _restApi.Post<TRequest, TResponse>(objectToPost));
        }

        /// <summary>
        /// Asynchronously sends a POST request with the specified object to the configured API endpoint, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPost">The object to be serialized and sent in the POST request body.</param>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="Task{RestResponse}"/> representing the asynchronous operation, containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy. 
        /// </remarks>
        public async Task<RestResponse<TResponse>> PostAsync<TRequest, TResponse>(TRequest objectToPost, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.PostAsync<TRequest, TResponse>(objectToPost, cancellationToken));
        }

        /// <summary>
        /// Sends a GET request to the configured API endpoint, implementing retry logic for transient failures.
        /// This method is marked as obsolete; it is recommended to use the asynchronous version for better performance and non-blocking behavior.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]

        public RestResponse<TResponse> Get<TResponse>() 
            where TResponse : new()
        {
            return Retry(() => _restApi.Get<TResponse>());
        }

        /// <summary>
        /// Asynchronously sends a GET request to the configured API endpoint, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <returns>A <see cref="Task{RestResponse}"/> representing the asynchronous operation, containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse<TResponse>> GetAsync<TResponse>(CancellationToken cancellationToken = default) 
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.GetAsync<TResponse>(cancellationToken));
        }

        /// <summary>
        /// Sends a GET request to the configured API endpoint with the specified query parameter, implementing retry logic for transient failures.
        /// This method is marked as obsolete; it is recommended to use the asynchronous version for better performance and non-blocking behavior.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="paramName">The name of the query parameter to include in the request.</param>
        /// <param name="paramValue">The value of the query parameter to include in the request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]

        public RestResponse<TResponse> Get<TResponse>(string paramName, string paramValue) 
            where TResponse : new()
        {
            return Retry(() => _restApi.Get<TResponse>(paramName, paramValue));
        }

        /// <summary>
        /// Asynchronously sends a GET request to the configured API endpoint with the specified query parameter, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="paramName">The name of the query parameter to include in the request.</param>
        /// <param name="paramValue">The value of the query parameter to include in the request.</param>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="Task{RestResponse}"/> representing the asynchronous operation, containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse<TResponse>> GetAsync<TResponse>(string paramName, string paramValue, CancellationToken cancellationToken = default) 
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.GetAsync<TResponse>(paramName, paramValue, cancellationToken));
        }

        /// <summary>
        /// Asynchronously sends a GET request to the configured API endpoint using a dictionary of query parameters, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="paramsKeyValue">A dictionary containing key-value pairs of query parameters to include in the request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method is marked as obsolete and should be replaced with the asynchronous version. It retries on transient failures as determined by the <see cref="RetryEngine"/> 
        /// and continues to retry until the response status code is accepted or the retry limit is reached. The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Get<TResponse>(Dictionary<string, string> paramsKeyValue) 
            where TResponse : new()
        {
            return Retry(() => _restApi.Get<TResponse>(paramsKeyValue));
        }

        /// <summary>
        /// Sends a PUT request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPut">The object to be sent in the PUT request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method is marked as obsolete and should be replaced with the asynchronous version. It will retry the request on transient failures as determined by the <see cref="RetryEngine"/>.
        /// The method continues to retry until the response status code is accepted or the maximum number of retries is reached.
        /// The delay between retries is determined by the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Put<TRequest, TResponse>(TRequest objectToPut) 
            where TRequest : class
            where TResponse : new()
        {
            return Retry(() => _restApi.Put<TRequest, TResponse>(objectToPut));
        }

        /// <summary>
        /// Asynchronously sends a PUT request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPut">The object to be sent in the PUT request.</param>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method is marked as obsolete and should be replaced with the asynchronous version. It retries on transient failures as determined by the <see cref="RetryEngine"/> 
        /// and continues to retry until the response status code is accepted or the retry limit is reached. The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse<TResponse>> PutAsync<TRequest, TResponse>(TRequest objectToPut, CancellationToken cancellationToken = default) 
            where TRequest : class
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.PutAsync<TRequest, TResponse>(objectToPut, cancellationToken));
        }

        /// <summary>
        /// Sends a DELETE request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToDelete">The object to be deleted via the DELETE request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method is marked as obsolete and should be replaced with the asynchronous version. It will retry the request on transient failures as determined by the <see cref="RetryEngine"/>.
        /// The method continues to retry until the response status code is accepted or the maximum number of retries is reached.
        /// The delay between retries is determined by the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>
        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Delete<TRequest, TResponse>(TRequest objectToDelete) 
            where TRequest : class
            where TResponse : new()
        {
            return Retry(() => _restApi.Delete<TRequest, TResponse>(objectToDelete));
        }

        /// <summary>
        /// Asynchronously sends a DELETE request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToDelete">The object to be deleted via the DELETE request.</param>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method performs an asynchronous operation and includes retry logic for transient failures as determined by the <see cref="RetryEngine"/>.
        /// The method will continue to retry until the response status code is accepted or the maximum number of retries is reached.
        /// The delay between retries is governed by the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse<TResponse>> DeleteAsync<TRequest, TResponse>(TRequest objectToDelete, CancellationToken cancellationToken = default) 
            where TRequest : class
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.DeleteAsync<TRequest, TResponse>(objectToDelete, cancellationToken));
        }

        /// <summary>
        /// Calculates the delay between retry attempts based on the specified backoff strategy.
        /// </summary>
        /// <param name="retry">The current retry attempt number, starting from zero.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the delay before the next retry attempt.</returns>
        /// <remarks>
        /// The delay is determined by the configured <see cref="BackoffTypes"/> strategy, which may include:
        /// <list type="bullet">
        ///     <item><description><see cref="BackoffTypes.Constant"/>: Returns a constant delay.</description></item>
        ///     <item><description><see cref="BackoffTypes.Linear"/>: Increases the delay linearly with each retry.</description></item>
        ///     <item><description><see cref="BackoffTypes.Exponential"/>: Doubles the delay for each retry attempt.</description></item>
        ///     <item><description><see cref="BackoffTypes.ExponentialWithJitter"/>: Applies jitter to the exponential backoff for better distribution of retry attempts.</description></item>
        ///     <item><description><see cref="BackoffTypes.Random"/>: Returns a random delay between a minimum and maximum value.</description></item>
        ///     <item><description><see cref="BackoffTypes.Fibonacci"/>: Uses Fibonacci sequence values to determine the delay.</description></item>
        ///     <item><description><see cref="BackoffTypes.ExponentialFullJitter"/>: Use Exponential Full Jitter for stateless retries that reduce collisions with randomized exponential delays.</description></item>
        /// </list>
        /// If an unrecognized <see cref="BackoffTypes"/> is provided, a constant delay is returned.
        /// </remarks>
        private TimeSpan GetDelay(int retry)
        {
            TimeSpan defaultMaxDelay = TimeSpan.FromSeconds(30);

            double delaySeconds;

            switch (DelayType)
            {
                case BackoffTypes.Constant:
                    {
                        delaySeconds = RetryDelay.TotalSeconds;
                        break;
                    }
                case BackoffTypes.Linear:
                    {
                        delaySeconds = RetryDelay.TotalSeconds * (retry + 1);
                        break;
                    }
                case BackoffTypes.Exponential:
                    {
                        delaySeconds = RetryDelay.TotalSeconds * Math.Pow(2, retry);
                        break;
                    }
                case BackoffTypes.ExponentialWithJitter:
                    {
                        delaySeconds = RetryDelay.TotalSeconds * Math.Pow(2, retry) * Random.Shared.NextDouble();
                        break;
                    }
                case BackoffTypes.Random:
                    {
                        double min = RetryDelay.TotalSeconds;
                        double max = RetryDelay.TotalSeconds * 2;
                        delaySeconds = Random.Shared.NextDouble() * (max - min) + min;
                        break;
                    }
                case BackoffTypes.Fibonacci:
                    {
                        int fibonacci = BackoffUtils.GetFibonacci(retry);
                        delaySeconds = RetryDelay.TotalSeconds * fibonacci;
                        break;
                    }
                case BackoffTypes.ExponentialFullJitter:
                    {
                        double maxDelay = RetryDelay.TotalSeconds * Math.Pow(2, retry);
                        double delayWithJitter = Random.Shared.NextDouble() * maxDelay;
                        delaySeconds = Math.Min(delayWithJitter, defaultMaxDelay.TotalSeconds);
                        break;
                    }
                default:
                    {
                        delaySeconds = RetryDelay.TotalSeconds;
                        break;
                    }
            }

            return TimeSpan.FromSeconds(Math.Min(delaySeconds, defaultMaxDelay.TotalSeconds));
        }

        /// <summary>
        /// Sends a PATCH request to update a resource with the provided object.
        /// This method is deprecated; use the asynchronous version instead.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <param name="objectToPatch">The object containing the data to update the resource.</param>
        /// <returns>A <see cref="RestResponse"/> containing the response from the PATCH request.</returns>
        /// <remarks>
        /// The method first attempts to send the PATCH request. If the response indicates a transient error, 
        /// it will retry the request up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which uses the current 
        /// <see cref="DelayType"/> strategy. The retry process continues until a successful response is received 
        /// or the maximum retry limit is reached.
        /// </remarks>

        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Patch<TRequest, TResponse>(TRequest objectToPatch) 
            where TRequest : class
            where TResponse : new()
        {
            return Retry(() => _restApi.Patch<TRequest, TResponse>(objectToPatch));
        }

        /// <summary>
        /// Asynchronously sends a PATCH request to update a resource with the provided object.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <param name="objectToPatch">The object containing the data to update the resource.</param>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="RestResponse"/> containing the response from the PATCH request.</returns>
        /// <remarks>
        /// The method first attempts to send the PATCH request asynchronously. If the response indicates a transient error, 
        /// it will retry the request up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which uses the current 
        /// <see cref="DelayType"/> strategy. The retry process continues until a successful response is received 
        /// (indicated by a status code of HttpStatusCode.Accepted) or the maximum retry limit is reached.
        /// </remarks>

        public async Task<RestResponse<TResponse>> PatchAsync<TRequest, TResponse>(TRequest objectToPatch, CancellationToken cancellationToken = default) 
            where TRequest : class
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.PatchAsync<TRequest, TResponse>(objectToPatch, cancellationToken));
        }

        /// <summary>
        /// Sends an OPTIONS request to the specified resource and retrieves the allowed HTTP methods.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <returns>A <see cref="RestResponse"/> containing the response from the OPTIONS request.</returns>
        /// <remarks>
        /// This method attempts to send an OPTIONS request synchronously to determine the allowed HTTP methods 
        /// for the specified resource. If the response indicates a transient error, it will retry the request 
        /// up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which applies the 
        /// current <see cref="DelayType"/> strategy. The retry process continues until a successful response is 
        /// received (indicated by a status code of HttpStatusCode.Accepted) or the maximum retry limit is reached.
        /// </remarks>

        [Obsolete("This method is deprecated. Use the asynchronous version instead.")]
        public RestResponse<TResponse> Options<TResponse>() 
            where TResponse : new()
        {
            return Retry(() => _restApi.Options<TResponse>());
        }

        /// <summary>
        /// Sends an asynchronous OPTIONS request to the specified resource to retrieve the allowed HTTP methods.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <param name="cancellationToken">The CancellationToken.</param>
        /// <returns>A <see cref="RestResponse"/> containing the response from the asynchronous OPTIONS request.</returns>
        /// <remarks>
        /// This method attempts to send an asynchronous OPTIONS request to determine the allowed HTTP methods 
        /// for the specified resource. If the response indicates a transient error, it will retry the request 
        /// up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which applies the 
        /// current <see cref="DelayType"/> strategy. The retry process continues until a successful response is 
        /// received (indicated by a status code of HttpStatusCode.Accepted) or the maximum retry limit is reached.
        /// </remarks>

        public async Task<RestResponse<TResponse>> OptionsAsync<TResponse>(CancellationToken cancellationToken = default) 
            where TResponse : new()
        {
            return await RetryAsync(() => _restApi.OptionsAsync<TResponse>(cancellationToken));
        }
    }
}
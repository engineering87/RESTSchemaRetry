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
        public RestResponse Post<T>(object objectToPost) where T : new()
        {       
            var response = _restApi.Post<T>(objectToPost);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Post<T>(objectToPost);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Asynchronously sends a POST request with the specified object to the configured API endpoint, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPost">The object to be serialized and sent in the POST request body.</param>
        /// <returns>A <see cref="Task{RestResponse}"/> representing the asynchronous operation, containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy. 
        /// </remarks>

        public async Task<RestResponse> PostAsync<T>(object objectToPost) where T : new()
        {          
            var response = await _restApi.PostAsync<T>(objectToPost);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.PostAsync<T>(objectToPost);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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

        public RestResponse Get<T>() where T : new()
        {
            var response = _restApi.Get<T>();

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Get<T>();
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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

        public async Task<RestResponse> GetAsync<T>() where T : new()
        {
            var response = await _restApi.GetAsync<T>();

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.GetAsync<T>();
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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

        public RestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var response = _restApi.Get<T>(paramName, paramValue);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Get<T>(paramName, paramValue);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Asynchronously sends a GET request to the configured API endpoint with the specified query parameter, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="paramName">The name of the query parameter to include in the request.</param>
        /// <param name="paramValue">The value of the query parameter to include in the request.</param>
        /// <returns>A <see cref="Task{RestResponse}"/> representing the asynchronous operation, containing the server's response.</returns>
        /// <remarks>
        /// This method retries on transient failures as determined by the <see cref="RetryEngine"/> and retries up to the specified limit. 
        /// The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse> GetAsync<T>(string paramName, string paramValue) where T : new()
        {
            var response = await _restApi.GetAsync<T>(paramName, paramValue);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.GetAsync<T>(paramName, paramValue);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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
        public RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var response = _restApi.Get<T>(paramsKeyValue);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Get<T>(paramsKeyValue);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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
        public RestResponse Put<T>(object objectToPut) where T : new()
        {
            var response = _restApi.Put<T>(objectToPut);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Put<T>(objectToPut);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Asynchronously sends a PUT request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToPut">The object to be sent in the PUT request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method is marked as obsolete and should be replaced with the asynchronous version. It retries on transient failures as determined by the <see cref="RetryEngine"/> 
        /// and continues to retry until the response status code is accepted or the retry limit is reached. The delay between retries follows the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse> PutAsync<T>(object objectToPut) where T : new()
        {
            var response = await _restApi.PutAsync<T>(objectToPut);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.PutAsync<T>(objectToPut);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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
        public RestResponse Delete<T>(object objectToDelete) where T : new()
        {
            var response = _restApi.Delete<T>(objectToDelete);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Delete<T>(objectToDelete);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Asynchronously sends a DELETE request to the configured API endpoint with the specified object, implementing retry logic for transient failures.
        /// </summary>
        /// <typeparam name="T">The expected response type, which must have a parameterless constructor.</typeparam>
        /// <param name="objectToDelete">The object to be deleted via the DELETE request.</param>
        /// <returns>A <see cref="RestResponse"/> containing the server's response.</returns>
        /// <remarks>
        /// This method performs an asynchronous operation and includes retry logic for transient failures as determined by the <see cref="RetryEngine"/>.
        /// The method will continue to retry until the response status code is accepted or the maximum number of retries is reached.
        /// The delay between retries is governed by the configured <see cref="BackoffTypes"/> strategy.
        /// </remarks>

        public async Task<RestResponse> DeleteAsync<T>(object objectToDelete) where T : new()
        {
            var response = await _restApi.DeleteAsync<T>(objectToDelete);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.DeleteAsync<T>(objectToDelete);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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
        /// </list>
        /// If an unrecognized <see cref="BackoffTypes"/> is provided, a constant delay is returned.
        /// </remarks>

        private TimeSpan GetDelay(int retry)
        {
            switch (DelayType)
            {
                case BackoffTypes.Constant:
                    return this.RetryDelay;
                case BackoffTypes.Linear:
                    {
                        var totalSeconds = this.RetryDelay.TotalSeconds * (retry + 1);
                        return TimeSpan.FromSeconds(totalSeconds);
                    }
                case BackoffTypes.Exponential:
                    {
                        var totalSeconds = (int)(this.RetryDelay.TotalSeconds * Math.Pow(2, retry));
                        return TimeSpan.FromSeconds(totalSeconds);
                    }
                case BackoffTypes.ExponentialWithJitter:
                    {
                        var random = new Random();
                        var jitter = random.NextDouble();
                        var totalSeconds = this.RetryDelay.TotalSeconds * Math.Pow(2, retry) * jitter;
                        return TimeSpan.FromSeconds(totalSeconds);
                    }
                case BackoffTypes.Random:
                    {
                        var random = new Random();
                        var minDelay = (int)this.RetryDelay.TotalSeconds;
                        var maxDelay = (int)(this.RetryDelay.TotalSeconds * 2);
                        var totalSeconds = random.Next(minDelay, maxDelay);
                        return TimeSpan.FromSeconds(totalSeconds);
                    }
                case BackoffTypes.Fibonacci:
                    {
                        int fibonacci = BackoffUtils.GetFibonacci(retry);
                        var totalSeconds = this.RetryDelay.TotalSeconds * fibonacci;
                        return TimeSpan.FromSeconds(totalSeconds);
                    }
                default:
                    return this.RetryDelay;
            }            
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
        public RestResponse Patch<T>(object objectToPatch) where T : new()
        {
            var response = _restApi.Patch<T>(objectToPatch);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Patch<T>(objectToPatch);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Asynchronously sends a PATCH request to update a resource with the provided object.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <param name="objectToPatch">The object containing the data to update the resource.</param>
        /// <returns>A <see cref="RestResponse"/> containing the response from the PATCH request.</returns>
        /// <remarks>
        /// The method first attempts to send the PATCH request asynchronously. If the response indicates a transient error, 
        /// it will retry the request up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which uses the current 
        /// <see cref="DelayType"/> strategy. The retry process continues until a successful response is received 
        /// (indicated by a status code of HttpStatusCode.Accepted) or the maximum retry limit is reached.
        /// </remarks>

        public async Task<RestResponse> PatchAsync<T>(object objectToPatch) where T : new()
        {
            var response = await _restApi.PatchAsync<T>(objectToPatch);

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.PatchAsync<T>(objectToPatch);
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
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
        public RestResponse Options<T>() where T : new()
        {
            var response = _restApi.Options<T>();

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Task.Delay(GetDelay(retry)).Wait();

                    response = _restApi.Options<T>();
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }

        /// <summary>
        /// Sends an asynchronous OPTIONS request to the specified resource to retrieve the allowed HTTP methods.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <returns>A <see cref="RestResponse"/> containing the response from the asynchronous OPTIONS request.</returns>
        /// <remarks>
        /// This method attempts to send an asynchronous OPTIONS request to determine the allowed HTTP methods 
        /// for the specified resource. If the response indicates a transient error, it will retry the request 
        /// up to a specified number of times, as defined by the <see cref="RetryNumber"/> property.
        /// The delay between retries is determined by the <see cref="GetDelay(int)"/> method, which applies the 
        /// current <see cref="DelayType"/> strategy. The retry process continues until a successful response is 
        /// received (indicated by a status code of HttpStatusCode.Accepted) or the maximum retry limit is reached.
        /// </remarks>

        public async Task<RestResponse> OptionsAsync<T>() where T : new()
        {
            var response = await _restApi.OptionsAsync<T>();

            if (!RetryEngine.Instance.IsTransient(response) || DelayType == BackoffTypes.NoRetry)
                return response;

            var retry = 0;
            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    await Task.Delay(GetDelay(retry));

                    response = await _restApi.OptionsAsync<T>();
                    retry++;
                }
                else
                {
                    break;
                }
            }

            return response;
        }
    }
}

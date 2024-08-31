// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Enum;
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

        private const int DefaultRetry = 3;
        private const int DefaultDelay = 5; // sec

        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        public RetryClient(string baseUrl, string resource)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, DefaultDelay);
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="retryDelayMs"></param>
        public RetryClient(string baseUrl, string resource, int retryDelayMs)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="backoffTypes"></param>
        public RetryClient(string baseUrl, string resource, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0, 0, 0, DefaultDelay);
            this.DelayType = backoffTypes;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="retryNumber"></param>
        /// <param name="retryDelayMs"></param>
        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="retryNumber"></param>
        /// <param name="retryDelayMs">Milliseconds</param>
        /// <param name="backoffTypes"></param>
        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = new TimeSpan(0, 0, 0, 0, retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = backoffTypes;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="retryNumber"></param>
        /// <param name="retryDelay"></param>
        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Constant;
        }

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="retryNumber"></param>
        /// <param name="retryDelay"></param>
        /// <param name="backoffTypes"></param>
        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = backoffTypes;
        }

        #endregion

        /// <summary>
        /// Execute POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
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
        /// Execute GET with no params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async GET with no params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
        /// Execute GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
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
        /// Execute GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsKeyValue"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
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
        /// Execute DELETE 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
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
        /// Compute the current step delay in seconds
        /// </summary>
        /// <param name="retry"></param>
        /// <returns></returns>
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
        /// Execute PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <returns></returns>
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
        /// Execute OPTIONS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                    Thread.Sleep(GetDelay(retry));

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
        /// Execute async OPTIONS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

// (c) 2019 engineering87
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Enum;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace RESTSchemaRetry
{
    /// <summary>
    /// ApiClient with Schema-Retry implementation
    /// </summary>
    public class RetryClient
    {
        private readonly RestApi _restApi;
        private readonly RetryEngine _retryEngine;
        public int RetryNumber { get; set; }
        public TimeSpan RetryDelay { get; set; }
        /// <summary>
        /// TODO differents delay types
        /// </summary>
        public BackoffTypes DelayType { get; set; }

        private const int DEFAULT_RETRY = 3;
        private const int DEFAULT_DELAY = 5;

        /// <summary>
        /// Default schema-retry configuration
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        public RetryClient(string baseUrl, string resource)
        {
            _restApi = new RestApi(baseUrl, resource);
            _retryEngine = new RetryEngine();
            // default value: max 3 retry every 5 seconds
            this.RetryNumber = DEFAULT_RETRY;
            this.RetryDelay = new TimeSpan(0,0,0, DEFAULT_DELAY);
            this.DelayType = BackoffTypes.Linear;
        }

        public RetryClient(string baseUrl, string resource, BackoffTypes backoffTypes)
        {
            _restApi = new RestApi(baseUrl, resource);
            _retryEngine = new RetryEngine();
            this.RetryNumber = DEFAULT_RETRY;
            this.RetryDelay = new TimeSpan(0, 0, 0, DEFAULT_DELAY);
            this.DelayType = backoffTypes;
        }

        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs)
        {
            _restApi = new RestApi(baseUrl, resource);
            _retryEngine = new RetryEngine();
            this.RetryDelay = new TimeSpan(0,0,0,0,retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Linear;
        }

        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay)
        {
            _restApi = new RestApi(baseUrl, resource);
            _retryEngine = new RetryEngine();
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Linear;
        }

        /// <summary>
        /// Execute POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
        public IRestResponse Post<T>(object objectToPost) where T : new()
        {
            var retry = 0;
            var response = _restApi.Post<T>(objectToPost);

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
        /// Execute GET with no params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRestResponse Get<T>() where T : new()
        {
            var retry = 0;
            var response = _restApi.Get<T>();

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
        /// Execute GET with paramater
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public IRestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var retry = 0;
            var response = _restApi.Get<T>(paramName, paramValue);

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
        /// Execute GET with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsKeyValue"></param>
        /// <returns></returns>
        public IRestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var retry = 0;
            var response = _restApi.Get<T>(paramsKeyValue);

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
        public IRestResponse Put<T>(object objectToPut) where T : new()
        {
            var retry = 0;
            var response = _restApi.Put<T>(objectToPut);

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
        /// Execute DELETE 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        public IRestResponse Delete<T>(object objectToDelete) where T : new()
        {
            var retry = 0;
            var response = _restApi.Delete<T>(objectToDelete);

            // check if the status code is transient
            if (!_retryEngine.IsTransient(response))
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

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
    }
}

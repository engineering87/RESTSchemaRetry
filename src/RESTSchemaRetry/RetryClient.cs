// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
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
    public sealed class RetryClient
    {
        private readonly RestApi _restApi;
        private readonly RetryEngine _retryEngine;
        public int RetryNumber { get; set; }
        public TimeSpan RetryDelay { get; set; }
        /// <summary>
        /// TODO differents delay types
        /// </summary>
        public BackoffTypes DelayType { get; set; }

        private const int DefaultRetry = 3;
        private const int DefaultDelay = 5;

        #region Constructors

        /// <summary>
        /// Create an instance of the RetryClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        public RetryClient(string baseUrl, string resource)
        {
            _restApi = new RestApi(baseUrl, resource);
            _retryEngine = new RetryEngine();
            this.RetryNumber = DefaultRetry;
            this.RetryDelay = new TimeSpan(0,0,0, DefaultDelay);
            this.DelayType = BackoffTypes.Linear;
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
            _retryEngine = new RetryEngine();
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
            _retryEngine = new RetryEngine();
            this.RetryDelay = new TimeSpan(0,0,0,0,retryDelayMs);
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Linear;
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
            _retryEngine = new RetryEngine();
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber >= 0 ? retryNumber : 0;
            this.DelayType = BackoffTypes.Linear;
        }

        #endregion

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

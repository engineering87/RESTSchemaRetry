using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using RESTSchemaRetry.Enum;

namespace RESTSchemaRetry
{
    /// <summary>
    /// ApiClient with Schema-Retry implementation
    /// </summary>
    public class RetryClient
    {
        private readonly RestApi restApi;
        public int RetryNumber { get; set; }
        public TimeSpan RetryDelay { get; set; }
        /// <summary>
        /// TODO differents delay types
        /// </summary>
        public DelayTypes DelayType { get; set; }

        /// <summary>
        /// Default schema-retry configuration.
        /// 3 retry every 5 seconds.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        public RetryClient(string baseUrl, string resource)
        {
            restApi = new RestApi(baseUrl, resource);
            // default value: max 3 retry every 5 seconds
            this.RetryNumber = 3;
            this.RetryDelay = new TimeSpan(0,0,0, 5);
            this.DelayType = DelayTypes.LINEAR;
        }

        public RetryClient(string baseUrl, string resource, int retryNumber, TimeSpan retryDelay)
        {
            restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = retryDelay;
            this.RetryNumber = retryNumber;
            this.DelayType = DelayTypes.LINEAR;
        }

        public RetryClient(string baseUrl, string resource, int retryNumber, int retryDelayMs)
        {
            restApi = new RestApi(baseUrl, resource);
            this.RetryDelay = new TimeSpan(0,0,0,0,retryDelayMs);
            this.RetryNumber = retryNumber;
            this.DelayType = DelayTypes.LINEAR;
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
            var response = restApi.Post<T>(objectToPost);

            // skip the NotFound status
            if (response.StatusCode == HttpStatusCode.NotFound)
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

                    response = restApi.Post<T>(objectToPost);
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
            var response = restApi.Get<T>();

            // skip the NotFound status
            if (response.StatusCode == HttpStatusCode.NotFound)
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

                    response = restApi.Get<T>();
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
            var response = restApi.Get<T>(paramName, paramValue);

            // skip the NotFound status
            if (response.StatusCode == HttpStatusCode.NotFound)
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

                    response = restApi.Get<T>(paramName, paramValue);
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
            var response = restApi.Get<T>(paramsKeyValue);

            // skip the NotFound status
            if (response.StatusCode == HttpStatusCode.NotFound)
                return response;

            while (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (retry <= this.RetryNumber)
                {
                    Thread.Sleep(this.RetryDelay);

                    response = restApi.Get<T>(paramsKeyValue);
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

// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using RESTSchemaRetry.Exceptions;
using RestSharp;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RESTSchemaRetry
{
    /// <summary>
    /// RestSharp wrapper class
    /// </summary>
    public sealed class RestApi
    {
        public readonly string BaseUrl;
        public readonly string Resource;

        public RestApi(string baseUrl, string resource)
        {
            CheckConfiguration(baseUrl, resource);

            BaseUrl = baseUrl;
            Resource = resource;
        }

        #region Checks

        private static void CheckConfiguration(string baseUrl, string resource)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(Messages.BaseUrlInvalid);

            if (string.IsNullOrEmpty(resource))
                throw new ArgumentNullException(Messages.ResourceInvalid);
        }

        private static void CheckObject(object objectBody)
        {
            if (objectBody == null)
            {
                throw new SerializationException(Messages.ObjectNull);
            }

            if (!objectBody.GetType().IsSerializable)
            {
                throw new SerializationException(Messages.SerializationError);
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Execute POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
        public IRestResponse Post<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return client.Execute<T>(request);
        }

        /// <summary>
        /// Execute async POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
        public async Task<IRestResponse> PostAsync<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return await client.ExecuteAsync<T>(request);
        }

        #endregion

        #region GET

        /// <summary>
        /// Execute GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRestResponse Get<T>() where T : new()
        {
            return Get<T>(string.Empty, string.Empty);
        }

        /// <summary>
        /// Execute async GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<IRestResponse> GetAsync<T>() where T : new()
        {
            return GetAsync<T>(string.Empty, string.Empty);
        }

        /// <summary>
        /// Execute GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public IRestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.GET);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return client.Execute<T>(request);
        }

        /// <summary>
        /// Execute async GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public async Task<IRestResponse> GetAsync<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.GET);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return await client.ExecuteAsync<T>(request);
        }

        /// <summary>
        /// Execute GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsKeyValue"></param>
        /// <returns></returns>
        public IRestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.GET);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return client.Execute<T>(request);
        }

        #endregion

        #region PUT

        /// <summary>
        /// Execute PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
        public IRestResponse Put<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return client.Execute<T>(request);
        }

        /// <summary>
        /// Execute async PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
        public async Task<IRestResponse> PutAsync<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return await client.ExecuteAsync<T>(request);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Execute DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        public IRestResponse Delete<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return client.Execute<T>(request);
        }

        /// <summary>
        /// Execute async DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        public async Task<IRestResponse> DeleteAsync<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return await client.ExecuteAsync<T>(request);
        }

        #endregion
    }
}

// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using RESTSchemaRetry.Exceptions;
using RestSharp;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using RESTSchemaRetry.Helper;
using RESTSchemaRetry.Interfaces;

namespace RESTSchemaRetry.Provider
{
    /// <summary>
    /// RestSharp wrapper class to RestSharp library
    /// </summary>
    public sealed class RestApi : IRestApi
    {
        public readonly string BaseUrl;
        public readonly string Resource;

        /// <summary>
        /// Create an instance of the RestApi wrapper
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
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
            {
                throw new ArgumentNullException(Messages.BaseUrlInvalid);
            }

            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(Messages.ResourceInvalid);
            }
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
        public RestResponse Post<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        /// <summary>
        /// Execute async POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
        public async Task<RestResponse> PostAsync<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Post)
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
        public RestResponse Get<T>() where T : new()
        {
            return Get<T>(string.Empty, string.Empty);
        }

        /// <summary>
        /// Execute async GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<RestResponse> GetAsync<T>() where T : new()
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
        public RestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        /// <summary>
        /// Execute async GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public async Task<RestResponse> GetAsync<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Get);

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
        public RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Get);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        #endregion

        #region PUT

        /// <summary>
        /// Execute PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
        public RestResponse Put<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        /// <summary>
        /// Execute async PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
        public async Task<RestResponse> PutAsync<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Put)
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
        public RestResponse Delete<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        /// <summary>
        /// Execute async DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        public async Task<RestResponse> DeleteAsync<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return await client.ExecuteAsync<T>(request);
        }

        #endregion

        #region PATCH

        /// <summary>
        /// Execute PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <returns></returns>
        public RestResponse Patch<T>(object objectToPatch) where T : new()
        {
            CheckObject(objectToPatch);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        /// <summary>
        /// Execute async PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <returns></returns>
        public async Task<RestResponse> PatchAsync<T>(object objectToPatch) where T : new()
        {
            CheckObject(objectToPatch);

            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return await client.ExecuteAsync<T>(request);
        }

        #endregion

        #region OPTION

        public RestResponse Options<T>() where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return AsyncHelper.RunSync(() => client.ExecuteAsync<T>(request));
        }

        public async Task<RestResponse> OptionsAsync<T>() where T : new()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return await client.ExecuteAsync<T>(request);
        }

        #endregion
    }
}

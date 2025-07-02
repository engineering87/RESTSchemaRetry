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
using System.Threading;

namespace RESTSchemaRetry.Provider
{
    /// <summary>
    /// RestSharp wrapper class to RestSharp library
    /// </summary>
    public class RestApi : IRestApi
    {
        private readonly RestClient _client;

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
            _client = new RestClient(BaseUrl);
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
        }

        #endregion

        #region POST

        /// <inheritdoc />
        public virtual RestResponse Post<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var request = new RestRequest(Resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> PostAsync<T>(object objectToPost, CancellationToken cancellationToken = default) where T : new()
        {
            CheckObject(objectToPost);

            var request = new RestRequest(Resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region GET

        /// <inheritdoc />
        public virtual RestResponse Get<T>() where T : new()
        {
            return Get<T>(string.Empty, string.Empty);
        }

        /// <inheritdoc />
        public virtual Task<RestResponse> GetAsync<T>(CancellationToken cancellationToken = default) where T : new()
        {
            return GetAsync<T>(string.Empty, string.Empty, cancellationToken);
        }

        /// <inheritdoc />
        public virtual RestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> GetAsync<T>(string paramName, string paramValue, CancellationToken cancellationToken = default) where T : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> GetAsync<T>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default) where T : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return await _client.ExecuteAsync<T>(request, cancellationToken);
        }

        #endregion

        #region PUT

        /// <inheritdoc />
        public virtual RestResponse Put<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var request = new RestRequest(Resource, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> PutAsync<T>(object objectToPut, CancellationToken cancellationToken = default) where T : new()
        {
            CheckObject(objectToPut);

            var request = new RestRequest(Resource, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region DELETE

        /// <inheritdoc />
        public virtual RestResponse Delete<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> DeleteAsync<T>(object objectToDelete, CancellationToken cancellationToken = default) where T : new()
        {
            CheckObject(objectToDelete);

            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region PATCH

        /// <inheritdoc />
        public virtual RestResponse Patch<T>(object objectToPatch) where T : new()
        {
            CheckObject(objectToPatch);

            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> PatchAsync<T>(object objectToPatch, CancellationToken cancellationToken = default) where T : new()
        {
            CheckObject(objectToPatch);

            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region OPTION

        /// <inheritdoc />
        public virtual RestResponse Options<T>() where T : new()
        {
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<T>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse> OptionsAsync<T>(CancellationToken cancellationToken = default) where T : new()
        {
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return await _client.ExecuteAsync<T>(request, cancellationToken: cancellationToken);
        }

        #endregion
    }
}

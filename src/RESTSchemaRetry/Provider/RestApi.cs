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
                throw new ArgumentException(Messages.BaseUrlInvalid);
            }

            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentException(Messages.ResourceInvalid);
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
        public virtual RestResponse<TResponse> Post<TRequest, TResponse>(TRequest objectToPost)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPost);

            var request = new RestRequest(Resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PostAsync<TRequest, TResponse>(TRequest objectToPost, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPost);

            var request = new RestRequest(Resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region GET

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Get<TResponse>() where TResponse : new()
        {
            return Get<TResponse>(string.Empty, string.Empty);
        }

        /// <inheritdoc />
        public virtual Task<RestResponse<TResponse>> GetAsync<TResponse>(CancellationToken cancellationToken = default) where TResponse : new()
        {
            return GetAsync<TResponse>(string.Empty, string.Empty, cancellationToken);
        }

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Get<TResponse>(string paramName, string paramValue) where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> GetAsync<TResponse>(string paramName, string paramValue, CancellationToken cancellationToken = default) where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Get<TResponse>(Dictionary<string, string> paramsKeyValue) where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> GetAsync<TResponse>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default) where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Get);

            if (paramsKeyValue != null)
            {
                foreach (var (key, value) in paramsKeyValue)
                {
                    request.AddParameter(key, value);
                }
            }

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken);
        }

        #endregion

        #region PUT

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Put<TRequest, TResponse>(TRequest objectToPut)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPut);

            var request = new RestRequest(Resource, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PutAsync<TRequest, TResponse>(TRequest objectToPut, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPut);

            var request = new RestRequest(Resource, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region DELETE

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Delete<TRequest, TResponse>(TRequest objectToDelete)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToDelete);

            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> DeleteAsync<TRequest, TResponse>(TRequest objectToDelete, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToDelete);

            var request = new RestRequest(Resource, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region PATCH

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Patch<TRequest, TResponse>(TRequest objectToPatch)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPatch);

            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PatchAsync<TRequest, TResponse>(TRequest objectToPatch, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPatch);

            var request = new RestRequest(Resource, Method.Patch)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPatch);

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        #endregion

        #region OPTIONS

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Options<TResponse>() where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> OptionsAsync<TResponse>(CancellationToken cancellationToken = default) 
            where TResponse : new()
        {
            var request = new RestRequest(Resource, Method.Options)
            {
                RequestFormat = DataFormat.Json
            };

            return await _client.ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken);
        }

        #endregion
    }
}

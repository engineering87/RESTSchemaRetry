// (c) 2019-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Exceptions;
using RESTSchemaRetry.Helper;
using RESTSchemaRetry.Interfaces;
using RESTSchemaRetry.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Provider
{
    /// <summary>
    /// General-purpose, strongly-typed HTTP client built on top of RestSharp, designed with
    /// resilience in mind. It centralizes base URL, resource path, auth, and default headers,
    /// and exposes sync/async methods for common HTTP verbs (GET, POST, PUT, PATCH, DELETE,
    /// OPTIONS, HEAD).
    /// </summary>
    /// <example>
    /// <code>
    /// var api = new RestApi("https://api.example.com", "/users", authToken: token);
    /// var res = await api.GetAsync<UserDto>(cancellationToken);
    /// if (res.IsSuccessful) { var user = res.Data; }
    /// </code>
    /// </example>
    public class RestApi : IRestApi, IDisposable
    {
        private readonly RestClient _client;

        public readonly string BaseUrl;
        public readonly string Resource;

        private readonly Dictionary<string, string> _defaultHeaders;
        private readonly string _authToken;

        /// <summary>
        /// Create an instance of the RestApi wrapper
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        public RestApi(string baseUrl, string resource)
        {
            (BaseUrl, Resource) = UrlNormalization.Normalize(baseUrl, resource);
            _client = new RestClient(new RestClientOptions(BaseUrl));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApi"/> class with optional authentication and custom headers.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API (e.g., "https://api.example.com").</param>
        /// <param name="resource">The resource endpoint path (e.g., "/users").</param>
        /// <param name="authToken">Optional Bearer token for authorization. If provided, it will be included in all requests.</param>
        /// <param name="defaultHeaders">Optional dictionary of custom headers to include in all requests.</param>
        public RestApi(string baseUrl, string resource, string authToken = null, Dictionary<string, string> defaultHeaders = null)
        {
            (BaseUrl, Resource) = UrlNormalization.Normalize(baseUrl, resource);
            _authToken = authToken;
            _defaultHeaders = defaultHeaders ?? [];

            _client = new RestClient(new RestClientOptions(BaseUrl));
        }

        #region Checks

        private static void CheckObject(object objectBody)
        {
            if (objectBody == null)
            {
                throw new SerializationException(Messages.ObjectNull);
            }
        }

        #endregion

        #region Create Request

        /// <summary>
        /// Creates a RestSharp request with optional JSON body, authentication, and default headers.
        /// </summary>
        /// <param name="method">The HTTP method to use (e.g., GET, POST, PUT, DELETE).</param>
        /// <param name="body">The object to be serialized as JSON and sent in the request body (optional).</param>
        /// <returns>A configured RestRequest object.</returns>
        private RestRequest CreateRequest(Method method, object body = null)
        {
            var request = new RestRequest(Resource, method);

            if (body != null)
                request.AddJsonBody(body);

            // If an authentication token is set, add it as a Bearer token in the Authorization header
            if (!string.IsNullOrEmpty(_authToken))
                request.AddHeader("Authorization", $"Bearer {_authToken}");

            // Add any default headers to the request, if they are defined
            if (_defaultHeaders != null)
            {
                foreach (var header in _defaultHeaders)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// Creates a request with optional dynamic resource, JSON body, auth and query parameters.
        /// </summary>
        private RestRequest CreateRequest(Method method, string resource, object body = null, Dictionary<string, string> queryParams = null)
        {
            var req = new RestRequest(string.IsNullOrWhiteSpace(resource) ? Resource : resource, method);

            if (body != null)
                req.AddJsonBody(body);

            if (queryParams != null)
                foreach (var (key, value) in queryParams)
                    req.AddQueryParameter(key, value);

            AddAuthAndHeaders(req);
            return req;
        }

        /// <summary>Adds Authorization (Bearer) and default headers to the request.</summary>
        /// <param name="request">Target <see cref="RestRequest"/>.</param>
        private void AddAuthAndHeaders(RestRequest request)
        {
            if (!string.IsNullOrEmpty(_authToken))
                request.AddHeader("Authorization", $"Bearer {_authToken}");

            if (_defaultHeaders != null)
                foreach (var header in _defaultHeaders)
                    request.AddHeader(header.Key, header.Value);
        }

        /// <summary>
        /// Creates a request with optional query parameters, authentication, and default headers.
        /// </summary>
        /// <param name="method">The HTTP method to use (e.g., GET, POST, PUT, DELETE).</param>
        /// <param name="queryParams">A dictionary of query parameter key-value pairs to be included in the request URL (optional).</param>
        /// <returns>A configured RestRequest object.</returns>
        private RestRequest CreateRequest(Method method, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(method);

            if (queryParams != null)
            {
                foreach (var (key, value) in queryParams)
                {
                    request.AddQueryParameter(key, value);
                }
            }

            return request;
        }

        #endregion

        #region POST

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Post<TRequest, TResponse>(TRequest objectToPost)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPost);

            var request = CreateRequest(Method.Post, objectToPost);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PostAsync<TRequest, TResponse>(TRequest objectToPost, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPost);

            var request = CreateRequest(Method.Post, objectToPost);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
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
            var request = CreateRequest(Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddQueryParameter(paramName, paramValue);
            }

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> GetAsync<TResponse>(string paramName, string paramValue, CancellationToken cancellationToken = default) where TResponse : new()
        {
            var request = CreateRequest(Method.Get);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddQueryParameter(paramName, paramValue);
            }

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Get<TResponse>(Dictionary<string, string> paramsKeyValue) where TResponse : new()
        {
            var request = CreateRequest(Method.Get, paramsKeyValue);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> GetAsync<TResponse>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default) where TResponse : new()
        {
            var request = CreateRequest(Method.Get, paramsKeyValue);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region PUT

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Put<TRequest, TResponse>(TRequest objectToPut)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPut);

            var request = CreateRequest(Method.Put, objectToPut);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PutAsync<TRequest, TResponse>(TRequest objectToPut, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPut);

            var request = CreateRequest(Method.Put, objectToPut);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region DELETE

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Delete<TRequest, TResponse>(TRequest objectToDelete)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToDelete);

            var request = CreateRequest(Method.Delete, objectToDelete);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> DeleteAsync<TRequest, TResponse>(TRequest objectToDelete, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToDelete);

            var request = CreateRequest(Method.Delete, objectToDelete);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region PATCH

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Patch<TRequest, TResponse>(TRequest objectToPatch)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPatch);

            var request = CreateRequest(Method.Patch, objectToPatch);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> PatchAsync<TRequest, TResponse>(TRequest objectToPatch, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new()
        {
            CheckObject(objectToPatch);

            var request = CreateRequest(Method.Patch, objectToPatch);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region OPTIONS

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Options<TResponse>() where TResponse : new()
        {
            var request = CreateRequest(Method.Options);

            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> OptionsAsync<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : new()
        {
            var request = CreateRequest(Method.Options);

            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region HEAD

        /// <inheritdoc />
        public virtual RestResponse<TResponse> Head<TResponse>(Dictionary<string, string> queryParams = null)
            where TResponse : new()
        {
            var request = CreateRequest(Method.Head, resource: null, body: null, queryParams: queryParams);
            return AsyncHelper.RunSync(() => _client.ExecuteAsync<TResponse>(request));
        }

        /// <inheritdoc />
        public virtual async Task<RestResponse<TResponse>> HeadAsync<TResponse>(
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
            where TResponse : new()
        {
            var request = CreateRequest(Method.Head, resource: null, body: null, queryParams: queryParams);
            return await _client
                .ExecuteAsync<TResponse>(request, cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
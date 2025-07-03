// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Interfaces
{
    public interface IRestApi
    {
        /// <summary>
        /// Execute synchronous POST request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPost">Object to post.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Post<TRequest, TResponse>(TRequest objectToPost) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous POST request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPost">Object to post.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> PostAsync<TRequest, TResponse>(TRequest objectToPost, CancellationToken cancellationToken = default) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute synchronous GET request without parameters.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Get<TResponse>()
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous GET request without parameters.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : new();

        /// <summary>
        /// Execute synchronous GET request with one parameter.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="paramValue">Parameter value.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Get<TResponse>(string paramName, string paramValue)
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous GET request with one parameter.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="paramValue">Parameter value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(string paramName, string paramValue, CancellationToken cancellationToken = default)
            where TResponse : new();

        /// <summary>
        /// Execute synchronous GET request with multiple parameters.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="paramsKeyValue">Dictionary of parameters.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Get<TResponse>(Dictionary<string, string> paramsKeyValue)
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous GET request with multiple parameters.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="paramsKeyValue">Dictionary of parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default)
            where TResponse : new();

        /// <summary>
        /// Execute synchronous PUT request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPut">Object to put.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Put<TRequest, TResponse>(TRequest objectToPut) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous PUT request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPut">Object to put.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> PutAsync<TRequest, TResponse>(TRequest objectToPut, CancellationToken cancellationToken = default) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute synchronous DELETE request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToDelete">Object to delete.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Delete<TRequest, TResponse>(TRequest objectToDelete) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous DELETE request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToDelete">Object to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> DeleteAsync<TRequest, TResponse>(TRequest objectToDelete, CancellationToken cancellationToken = default) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute synchronous PATCH request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPatch">Object to patch.</param>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Patch<TRequest, TResponse>(TRequest objectToPatch) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous PATCH request.
        /// </summary>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="objectToPatch">Object to patch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> PatchAsync<TRequest, TResponse>(TRequest objectToPatch, CancellationToken cancellationToken = default) where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Execute synchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <returns>RestResponse.</returns>
        RestResponse<TResponse> Options<TResponse>()
            where TResponse : new();

        /// <summary>
        /// Execute asynchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse<TResponse>> OptionsAsync<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : new();
    }
}
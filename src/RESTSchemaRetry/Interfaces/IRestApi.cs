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
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPost">Object to post.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Post<T>(object objectToPost) where T : new();

        /// <summary>
        /// Execute asynchronous POST request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPost">Object to post.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> PostAsync<T>(object objectToPost, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous GET request without parameters.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <returns>RestResponse.</returns>
        RestResponse Get<T>() where T : new();

        /// <summary>
        /// Execute asynchronous GET request without parameters.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> GetAsync<T>(CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous GET request with one parameter.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="paramValue">Parameter value.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Get<T>(string paramName, string paramValue) where T : new();

        /// <summary>
        /// Execute asynchronous GET request with one parameter.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="paramValue">Parameter value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> GetAsync<T>(string paramName, string paramValue, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous GET request with multiple parameters.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="paramsKeyValue">Dictionary of parameters.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new();

        /// <summary>
        /// Execute asynchronous GET request with multiple parameters.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="paramsKeyValue">Dictionary of parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> GetAsync<T>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous PUT request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPut">Object to put.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Put<T>(object objectToPut) where T : new();

        /// <summary>
        /// Execute asynchronous PUT request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPut">Object to put.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> PutAsync<T>(object objectToPut, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous DELETE request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToDelete">Object to delete.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Delete<T>(object objectToDelete) where T : new();

        /// <summary>
        /// Execute asynchronous DELETE request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToDelete">Object to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> DeleteAsync<T>(object objectToDelete, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous PATCH request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPatch">Object to patch.</param>
        /// <returns>RestResponse.</returns>
        RestResponse Patch<T>(object objectToPatch) where T : new();

        /// <summary>
        /// Execute asynchronous PATCH request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="objectToPatch">Object to patch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> PatchAsync<T>(object objectToPatch, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute synchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <returns>RestResponse.</returns>
        RestResponse Options<T>() where T : new();

        /// <summary>
        /// Execute asynchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RestResponse task.</returns>
        Task<RestResponse> OptionsAsync<T>(CancellationToken cancellationToken = default) where T : new();
    }
}

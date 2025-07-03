// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Interfaces
{
    public interface IRetryClient
    {
        /// <summary>
        /// Executes a synchronous POST request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to post.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPost">The object to send in the POST request body.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Post<TRequest, TResponse>(TRequest objectToPost)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous POST request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to post.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPost">The object to send in the POST request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> PostAsync<TRequest, TResponse>(TRequest objectToPost, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous GET request with no parameters.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Get<TResponse>()
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous GET request with no parameters.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous GET request with a single query parameter.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="paramName">The name of the query parameter.</param>
        /// <param name="paramValue">The value of the query parameter.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Get<TResponse>(string paramName, string paramValue)
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous GET request with a single query parameter.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="paramName">The name of the query parameter.</param>
        /// <param name="paramValue">The value of the query parameter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(string paramName, string paramValue, CancellationToken cancellationToken = default)
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous GET request with multiple query parameters.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="paramsKeyValue">A dictionary of query parameter key-value pairs.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Get<TResponse>(Dictionary<string, string> paramsKeyValue)
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous PUT request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to put.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPut">The object to send in the PUT request body.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Put<TRequest, TResponse>(TRequest objectToPut)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous PUT request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to put.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPut">The object to send in the PUT request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> PutAsync<TRequest, TResponse>(TRequest objectToPut, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous DELETE request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to delete.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToDelete">The object to send in the DELETE request body.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Delete<TRequest, TResponse>(TRequest objectToDelete)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous DELETE request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to delete.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToDelete">The object to send in the DELETE request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> DeleteAsync<TRequest, TResponse>(TRequest objectToDelete, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous PATCH request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to patch.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPatch">The object to send in the PATCH request body.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Patch<TRequest, TResponse>(TRequest objectToPatch)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous PATCH request with a request body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to patch.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="objectToPatch">The object to send in the PATCH request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> PatchAsync<TRequest, TResponse>(TRequest objectToPatch, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : new();

        /// <summary>
        /// Executes a synchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing the response.</returns>
        RestResponse<TResponse> Options<TResponse>()
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous OPTIONS request.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> OptionsAsync<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : new();
    }
}
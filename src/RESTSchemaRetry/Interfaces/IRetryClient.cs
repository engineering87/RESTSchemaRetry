// (c) 2019-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Interfaces
{
    /// <summary>
    /// Abstraction for an HTTP client with retry/resilience capabilities.
    /// 
    /// This interface mirrors common HTTP verbs (GET, POST, PUT, PATCH, DELETE, OPTIONS) and
    /// provides asynchronous counterparts. It also exposes HEAD for metadata checks and two
    /// generic "send" methods to support uncommon verbs or dynamic resource paths.
    /// 
    /// <remarks>
    /// <para>
    /// <b>Resilience:</b> An implementation may wrap calls with retry/circuit-breaker policies
    /// (e.g., using Polly). Transient failures (timeouts, 5xx, network faults) are typically
    /// candidates for retries, whereas client errors (4xx) should not be retried.
    /// </para>
    /// <para>
    /// <b>Serialization:</b> Methods using <c>TRequest</c> assume JSON serialization for the body,
    /// and methods using <c>TResponse</c> assume JSON deserialization of the response content.
    /// </para>
    /// <para>
    /// <b>Cancellation:</b> Async methods accept a <see cref="CancellationToken"/> to cancel
    /// in-flight HTTP operations.
    /// </para>
    /// </remarks>
    /// </summary>
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
        /// Executes an asynchronous GET request with multiple query parameters.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="paramsKeyValue">A dictionary of query parameter key-value pairs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="RestResponse{TResponse}"/> result.</returns>
        Task<RestResponse<TResponse>> GetAsync<TResponse>(Dictionary<string, string> paramsKeyValue, CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Executes a synchronous HEAD request.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The expected response type. For HEAD requests, servers normally return no body;
        /// therefore <c>TResponse</c> will typically be unused (default value) and you will
        /// inspect status code and headers in <see cref="RestResponse{TResponse}"/>.
        /// </typeparam>
        /// <param name="queryParams">Optional query string parameters appended to the request URL.</param>
        /// <returns>A <see cref="RestResponse{TResponse}"/> containing status and headers.</returns>
        /// <remarks>
        /// Use HEAD to check resource existence, preconditions, caching headers (ETag/Last-Modified),
        /// or content length without downloading the payload.
        /// </remarks>
        RestResponse<TResponse> Head<TResponse>(Dictionary<string, string> queryParams = null)
            where TResponse : new();

        /// <summary>
        /// Executes an asynchronous HEAD request.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The expected response type. For HEAD requests, servers normally return no body;
        /// therefore <c>TResponse</c> will typically be unused (default value) and you will
        /// inspect status code and headers in <see cref="RestResponse{TResponse}"/>.
        /// </typeparam>
        /// <param name="queryParams">Optional query string parameters appended to the request URL.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task with a <see cref="RestResponse{TResponse}"/> containing status and headers.</returns>
        /// <remarks>
        /// Use HEAD to validate links, check authorization, or retrieve caching metadata before issuing a GET.
        /// </remarks>
        Task<RestResponse<TResponse>> HeadAsync<TResponse>(Dictionary<string, string> queryParams = null, CancellationToken cancellationToken = default)
            where TResponse : new();
    }
}
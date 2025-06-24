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
        /// Execute POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <returns></returns>
        RestResponse Post<T>(object objectToPost) where T : new();

        /// <summary>
        /// Execute async POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPost"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> PostAsync<T>(object objectToPost, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute GET with no params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        RestResponse Get<T>() where T : new();

        /// <summary>
        /// Execute async GET with no params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> GetAsync<T>(CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        RestResponse Get<T>(string paramName, string paramValue) where T : new();

        /// <summary>
        /// Execute async GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> GetAsync<T>(string paramName, string paramValue, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute GET with params
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsKeyValue"></param>
        /// <returns></returns>
        RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new();

        /// <summary>
        /// Execute PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <returns></returns>
        RestResponse Put<T>(object objectToPut) where T : new();

        /// <summary>
        /// Execute async PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPut"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> PutAsync<T>(object objectToPut, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute DELETE 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        RestResponse Delete<T>(object objectToDelete) where T : new();

        /// <summary>
        /// Execute async DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDelete"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> DeleteAsync<T>(object objectToDelete, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <returns></returns>
        RestResponse Patch<T>(object objectToPatch) where T : new();

        /// <summary>
        /// Execute async PATCH
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToPatch"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> PatchAsync<T>(object objectToPatch, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Execute OPTIONS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        RestResponse Options<T>() where T : new();

        /// <summary>
        /// Execute async OPTIONS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<RestResponse> OptionsAsync<T>(CancellationToken cancellationToken = default) where T : new();
    }
}

// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Interfaces
{
    public interface IRestApi
    {
        RestResponse Post<T>(object objectToPost) where T : new();
        Task<RestResponse> PostAsync<T>(object objectToPost) where T : new();
        RestResponse Get<T>() where T : new();
        Task<RestResponse> GetAsync<T>() where T : new();
        RestResponse Get<T>(string paramName, string paramValue) where T : new();
        Task<RestResponse> GetAsync<T>(string paramName, string paramValue) where T : new();
        RestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new();      RestResponse Put<T>(object objectToPut) where T : new();
        Task<RestResponse> PutAsync<T>(object objectToPut) where T : new();
        RestResponse Delete<T>(object objectToDelete) where T : new();
        Task<RestResponse> DeleteAsync<T>(object objectToDelete) where T : new();
        RestResponse Patch<T>(object objectToPatch) where T : new();
        Task<RestResponse> PatchAsync<T>(object objectToPatch) where T : new();
        RestResponse Options<T>() where T : new();
        Task<RestResponse> OptionsAsync<T>() where T : new();
    }
}

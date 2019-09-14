// (c) 2019 engineering87
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using RESTSchemaRetry.Exceptions;
using RestSharp;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RESTSchemaRetry
{
    public sealed class RestApi
    {
        public string BaseUrl { get; }
        public string Resource { get; }

        public RestApi(string baseUrl, string resource)
        {
            CheckConfiguration(baseUrl, resource);

            this.BaseUrl = baseUrl;
            this.Resource = resource;
        }

        private static void CheckConfiguration(string baseUrl, string resource)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(Messages.BaseUrlInvalid);

            if (string.IsNullOrEmpty(resource))
                throw new ArgumentNullException(Messages.ResourceInvalid);
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

        #region POST

        public IRestResponse Post<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return client.Execute<T>(request);
        }

        public async Task<IRestResponse> PostAsync<T>(object objectToPost) where T : new()
        {
            CheckObject(objectToPost);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPost);

            return await client.ExecuteTaskAsync<T>(request);
        }

        #endregion

        #region GET

        public IRestResponse Get<T>() where T : new()
        {
            return Get<T>(string.Empty, string.Empty);
        }

        public Task<IRestResponse> GetAsync<T>() where T : new()
        {
            return GetAsync<T>(string.Empty, string.Empty);
        }

        public IRestResponse Get<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.GET);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return client.Execute<T>(request);
        }

        public async Task<IRestResponse> GetAsync<T>(string paramName, string paramValue) where T : new()
        {
            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.GET);

            if (!string.IsNullOrEmpty(paramName))
            {
                request.AddParameter(paramName, paramValue);
            }

            return await client.ExecuteTaskAsync<T>(request);
        }

        public IRestResponse Get<T>(Dictionary<string, string> paramsKeyValue) where T : new()
        {
            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.GET);

            if (paramsKeyValue != null)
            {
                foreach (var param in paramsKeyValue)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }

            return client.Execute<T>(request);
        }

        #endregion

        #region PUT

        public IRestResponse Put<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return client.Execute<T>(request);
        }

        public async Task<IRestResponse> PutAsync<T>(object objectToPut) where T : new()
        {
            CheckObject(objectToPut);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToPut);

            return await client.ExecuteTaskAsync<T>(request);
        }

        #endregion

        #region DELETE

        public IRestResponse Delete<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return client.Execute<T>(request);
        }

        public async Task<IRestResponse> DeleteAsync<T>(object objectToDelete) where T : new()
        {
            CheckObject(objectToDelete);

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(objectToDelete);

            return await client.ExecuteTaskAsync<T>(request);
        }

        #endregion
    }
}

using System;
using RESTSchemaRetry.Exceptions;
using RestSharp;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RESTSchemaRetry
{
    public class RestApi
    {
        public string BaseUrl { get; }
        public string Resource { get; }

        public RestApi(string baseUrl, string resource)
        {
            CheckConfiguration(baseUrl, resource);

            this.BaseUrl = baseUrl;
            this.Resource = resource;
        }

        private void CheckConfiguration(string baseUrl, string resource)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(Messages.BASEURL_INVALID);

            if (string.IsNullOrEmpty(resource))
                throw new ArgumentNullException(Messages.RESOURCE_INVALID);
        }

        public IRestResponse Post<T>(object objectToPost) where T : new()
        {
            if (!objectToPost.GetType().IsSerializable)
            {
                throw new SerializationException(Messages.SERIALIZATION_ERROR);
            }

            var client = new RestClient(this.BaseUrl);
            var request = new RestRequest(this.Resource, Method.POST);

            request.AddObject(objectToPost);

            return client.Execute<T>(request);
        }

        public IRestResponse Get<T>() where T : new()
        {
            return Get<T>(string.Empty, string.Empty);
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
    }
}

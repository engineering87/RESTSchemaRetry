// (c) 2019 engineering87
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Linq;
using System.Net;
using RestSharp;

namespace RESTSchemaRetry
{
    public class RetryEngine
    {
        /// <summary>
        /// Determine if the response status code is transient
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public bool IsTransient(HttpStatusCode statusCode)
        {
            return !new[] {
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.HttpVersionNotSupported,
                    HttpStatusCode.LengthRequired,
                    HttpStatusCode.UnsupportedMediaType
            }.Contains(statusCode);
        }

        /// <summary>
        /// Determine if the response status code is transient
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool IsTransient(IRestResponse response)
        {
            return IsTransient(response.StatusCode);
        }
    }
}

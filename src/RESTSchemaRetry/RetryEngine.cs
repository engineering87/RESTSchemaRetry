// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Linq;
using System.Net;
using RestSharp;

namespace RESTSchemaRetry
{
    public sealed class RetryEngine
    {
        private static RetryEngine instance;
        private static readonly object padlock = new();

        /// <summary>
        /// The Http status codes deemed non-transient
        /// </summary>
        private readonly HttpStatusCode[] httpStatusCode;

        private RetryEngine()
        {
            httpStatusCode = new[] {
                    HttpStatusCode.TooManyRequests,
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.BadGateway,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.GatewayTimeout,
                    HttpStatusCode.InsufficientStorage,
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.HttpVersionNotSupported,
                    HttpStatusCode.NetworkAuthenticationRequired
            };
        }

        public static RetryEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new RetryEngine();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Determine if the response http status code is transient
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public bool IsTransient(HttpStatusCode statusCode)
        {
            return !httpStatusCode.Contains(statusCode);
        }

        /// <summary>
        /// Determine if the response status code is transient
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool IsTransient(RestResponse response)
        {
            return IsTransient(response.StatusCode);
        }
    }
}

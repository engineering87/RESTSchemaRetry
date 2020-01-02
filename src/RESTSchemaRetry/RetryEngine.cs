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
        private static readonly object padlock = new object();

        /// <summary>
        /// The Http status codes deemed non-transient
        /// </summary>
        private readonly HttpStatusCode[] httpStatusCode;

        private RetryEngine()
        {
            httpStatusCode = new[] {
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.HttpVersionNotSupported,
                    HttpStatusCode.LengthRequired,
                    HttpStatusCode.UnsupportedMediaType,
                    HttpStatusCode.ProxyAuthenticationRequired,
                    HttpStatusCode.Gone,
                    HttpStatusCode.LengthRequired,
                    HttpStatusCode.PreconditionFailed,
                    HttpStatusCode.RequestEntityTooLarge,
                    HttpStatusCode.RequestUriTooLong,
                    HttpStatusCode.RequestedRangeNotSatisfiable
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
        public bool IsTransient(IRestResponse response)
        {
            return IsTransient(response.StatusCode);
        }
    }
}

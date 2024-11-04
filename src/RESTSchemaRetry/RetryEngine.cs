// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Linq;
using System.Net;
using RestSharp;

namespace RESTSchemaRetry
{
    public sealed class RetryEngine
    {
        private static readonly Lazy<RetryEngine> lazyInstance =
            new(() => new RetryEngine());

        public static RetryEngine Instance => lazyInstance.Value;

        /// <summary>
        /// The Http status codes deemed non-transient
        /// </summary>
        private readonly HttpStatusCode[] httpStatusCode;

        private RetryEngine()
        {
            // list of the transient error codes
            httpStatusCode = [
                    HttpStatusCode.TooManyRequests,
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.BadGateway,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.GatewayTimeout,
                    HttpStatusCode.InsufficientStorage,
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.HttpVersionNotSupported,
                    HttpStatusCode.NetworkAuthenticationRequired
            ];
        }

        /// <summary>
        /// Determine if the response HTTP status code is transient.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to check.</param>
        /// <returns>True if the status code is transient; otherwise, false.</returns>
        public bool IsTransient(HttpStatusCode statusCode)
        {
            return httpStatusCode.Contains(statusCode);
        }

        /// <summary>
        /// Determine if the response status code is transient.
        /// </summary>
        /// <param name="response">The REST response to check.</param>
        /// <returns>True if the response status code is transient; otherwise, false.</returns>
        public bool IsTransient(RestResponse response)
        {
            return IsTransient(response.StatusCode);
        }
    }
}

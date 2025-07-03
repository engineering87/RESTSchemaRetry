// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace RESTSchemaRetry
{
    public static class RetryEngine
    {
        /// <summary>
        /// The set of HTTP status codes considered transient errors.
        /// </summary>
        private static readonly HashSet<HttpStatusCode> transientStatusCodes =
        [
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

        /// <summary>
        /// Determine if the response HTTP status code is transient.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to check.</param>
        /// <returns>True if the status code is transient; otherwise, false.</returns>
        public static bool IsTransientStatusCode(HttpStatusCode statusCode)
        {
            return transientStatusCodes.Contains(statusCode);
        }

        /// <summary>
        /// Determine if the response status code is transient.
        /// </summary>
        /// <param name="response">The REST response to check.</param>
        /// <returns>True if the response status code is transient; otherwise, false.</returns>
        public static bool IsTransientStatusCode(RestResponse response)
        {
            if (response is null)
                return false;

            return IsTransientStatusCode(response.StatusCode);
        }
    }
}

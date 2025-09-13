// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace RESTSchemaRetry.Utils
{
    internal static class UrlNormalization
    {
        /// <summary>
        /// Validates the <paramref name="baseUrl"/> as an absolute URI and normalizes 
        /// the <paramref name="resource"/> so that it always starts with '/'.
        /// </summary>
        /// <param name="baseUrl">The base URL to validate (must be an absolute URI).</param>
        /// <param name="resource">The resource path to normalize.</param>
        /// <returns>
        /// A tuple containing the validated <see cref="baseUrl"/> and the normalized <see cref="resource"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="baseUrl"/> is not a valid absolute URI 
        /// or if <paramref name="resource"/> is null or empty.
        /// </exception>
        public static (string BaseUrl, string Resource) Normalize(string baseUrl, string resource)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Base URL must be a valid absolute URI.", nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));

            var normalizedResource = resource.StartsWith("/") ? resource : "/" + resource;
            return (baseUrl, normalizedResource);
        }

        /// <summary>
        /// Normalizes only the <paramref name="resource"/> string 
        /// (useful when overriding the default resource path at runtime).
        /// </summary>
        /// <param name="resource">The resource path to normalize.</param>
        /// <returns>The normalized resource, guaranteed to start with '/'.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="resource"/> is null or empty.
        /// </exception>
        public static string NormalizeResource(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));

            return resource.StartsWith("/") ? resource : "/" + resource;
        }
    }
}
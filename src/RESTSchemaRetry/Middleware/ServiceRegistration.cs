// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using RESTSchemaRetry.Interfaces;
using System;
using System.Collections.Generic;

namespace RESTSchemaRetry.Middleware
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Registers the <see cref="IRetryClient"/> service in the DI container with specified base URL and resource.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to register services.</param>
        /// <param name="baseUrl">The base URL for the API client (must be a valid URL).</param>
        /// <param name="resource">The resource endpoint for the API client.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="baseUrl"/> or <paramref name="resource"/> is null or empty.</exception>
        public static IServiceCollection AddRetryClient(this IServiceCollection services, string baseUrl, string resource)
        {
            ValidateBaseUrlAndResource(baseUrl, resource, out var normalizedResource);

            services.AddScoped<IRetryClient>(provider => new RetryClient(baseUrl, normalizedResource));

            return services;
        }

        /// <summary>
        /// Registers IRetryClient/IRestApi with optional Bearer token and default headers.
        /// </summary>
        /// <param name="services">The DI service collection.</param>
        /// <param name="baseUrl">Absolute base URL for the API.</param>
        /// <param name="resource">Default resource path.</param>
        /// <param name="authToken">Optional Bearer token applied to all requests.</param>
        /// <param name="defaultHeaders">Optional default headers applied to all requests.</param>
        /// <returns>The same <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRetryClient(
            this IServiceCollection services,
            string baseUrl,
            string resource,
            string authToken,
            IDictionary<string, string> defaultHeaders = null)
        {
            ValidateBaseUrlAndResource(baseUrl, resource, out var normalizedResource);

            var headersCopy = defaultHeaders is null ? null : new Dictionary<string, string>(defaultHeaders);

            services.AddScoped<IRetryClient>(provider => new RetryClient(baseUrl, normalizedResource, authToken, headersCopy));

            return services;
        }

        /// <summary>
        /// Ensures the base URL is an absolute URI and the resource is non-empty;
        /// normalizes the resource to start with '/'.
        /// </summary>
        private static void ValidateBaseUrlAndResource(string baseUrl, string resource, out string normalizedResource)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Base URL must be a valid absolute URI.", nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));

            normalizedResource = resource.StartsWith("/") ? resource : "/" + resource;
        }
    }
}

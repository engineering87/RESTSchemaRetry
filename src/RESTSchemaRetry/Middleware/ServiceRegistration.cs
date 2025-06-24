// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using RESTSchemaRetry.Interfaces;
using System;

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
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));

            services.AddScoped<IRetryClient>(provider => new RetryClient(baseUrl, resource));

            return services;
        }
    }
}

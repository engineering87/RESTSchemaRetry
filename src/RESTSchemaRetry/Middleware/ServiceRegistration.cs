// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using RESTSchemaRetry.Interfaces;

namespace RESTSchemaRetry.Middleware
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Extension method to register the IRetryClient service in the Dependency Injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection instance used to register services.</param>
        /// <param name="baseUrl">The base URL for the RestSharp client.</param>
        /// <param name="resource">The resourse for the RestSharp client.</param>
        /// <returns>IServiceCollection for fluent API usage.</returns>
        public static IServiceCollection AddRetryClient(this IServiceCollection services, string baseUrl, string resource)
        {
            services.AddScoped<IRetryClient>(provider => new RetryClient(baseUrl, resource));

            return services;
        }
    }
}

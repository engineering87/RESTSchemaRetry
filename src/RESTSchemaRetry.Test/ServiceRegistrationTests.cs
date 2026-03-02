// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using RESTSchemaRetry.Interfaces;
using RESTSchemaRetry.Middleware;

namespace RESTSchemaRetry.Test
{
    public class ServiceRegistrationTests
    {
        private readonly string _baseUrl = "https://api.example.com";
        private readonly string _resource = "/test";

        [Fact]
        public void AddRetryClient_RegistersIRetryClient()
        {
            var services = new ServiceCollection();

            services.AddRetryClient(_baseUrl, _resource);

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var client = scope.ServiceProvider.GetService<IRetryClient>();

            Assert.NotNull(client);
            Assert.IsType<RetryClient>(client);
        }

        [Fact]
        public void AddRetryClient_WithAuthToken_RegistersIRetryClient()
        {
            var services = new ServiceCollection();

            services.AddRetryClient(_baseUrl, _resource, "my-token");

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var client = scope.ServiceProvider.GetService<IRetryClient>();

            Assert.NotNull(client);
        }

        [Fact]
        public void AddRetryClient_InvalidBaseUrl_ThrowsArgumentException()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentException>(() =>
                services.AddRetryClient("not-a-url", _resource));
        }

        [Fact]
        public void AddRetryClient_EmptyResource_ThrowsArgumentException()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentException>(() =>
                services.AddRetryClient(_baseUrl, ""));
        }
    }
}

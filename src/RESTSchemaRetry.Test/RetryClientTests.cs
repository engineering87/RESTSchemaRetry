// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Moq;
using RESTSchemaRetry.Enum;
using RESTSchemaRetry.Provider;
using RESTSchemaRetry.Test.Model;
using RestSharp;
using System.Net;
using System.Reflection;

namespace RESTSchemaRetry.Test
{
    public class RetryClientTests
    {
        private readonly string _baseUrl = "https://api.example.com";
        private readonly string _resource = "/test";

        private RetryClient CreateClientWithMockApi(Mock<RestApi> mockApi)
        {
            var client = new RetryClient(_baseUrl, _resource)
            {
                RetryNumber = 3,
                RetryDelay = TimeSpan.Zero,
                DelayType = BackoffTypes.Constant
            };

            // Override private readonly field _restApi using reflection
            var restApiField = typeof(RetryClient).GetField("_restApi", BindingFlags.NonPublic | BindingFlags.Instance);
            restApiField?.SetValue(client, mockApi.Object);

            return client;
        }

        [Fact]
        public async Task PostAsync_ShouldNotRetry_OnSuccessResponse()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            mockApi
                .Setup(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<DummyResponse>(new RestRequest())
                {
                    StatusCode = HttpStatusCode.Accepted,
                    Data = new DummyResponse()
                });

            var client = CreateClientWithMockApi(mockApi);

            // Act
            var requestPayload = new DummyRequest { };
            var result = await client.PostAsync<DummyRequest, DummyResponse>(requestPayload);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
            mockApi.Verify(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PostAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            // Arrange
            var responses = new Queue<RestResponse<DummyResponse>>(new[]
            {
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.Accepted, Data = new DummyResponse() }
            });

            mockApi
                .Setup(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            // Act
            var requestPayload = new DummyRequest(); // Puoi aggiungere dati se vuoi
            var response = await client.PostAsync<DummyRequest, DummyResponse>(requestPayload);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            mockApi.Verify(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public void Constructor_DefaultValuesAreSet()
        {
            var client = new RetryClient(_baseUrl, _resource);

            Assert.Equal(1, client.RetryNumber);
            Assert.Equal(TimeSpan.FromSeconds(5), client.RetryDelay);
            Assert.Equal(BackoffTypes.Constant, client.DelayType);
        }

        [Fact]
        public void Constructor_CustomRetryDelayIsSet()
        {
            int customDelay = 2000; // in milliseconds
            var client = new RetryClient(_baseUrl, _resource, customDelay);

            Assert.Equal(TimeSpan.FromMilliseconds(customDelay), client.RetryDelay);
        }

        [Fact]
        public void Constructor_NegativeRetryDelay_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryClient(_baseUrl, _resource, -100));
        }

        [Fact]
        public void Constructor_NegativeRetryNumber_DefaultsToOne()
        {
            var client = new RetryClient(_baseUrl, _resource, -5, 1000);
            Assert.Equal(1, client.RetryNumber);
        }

        [Fact]
        public void TestTransientHttpCheck()
        {
            var isTransient = RetryEngine.IsTransientStatusCode(HttpStatusCode.BadRequest);
            Assert.False(isTransient);
            isTransient = RetryEngine.IsTransientStatusCode(HttpStatusCode.GatewayTimeout);
            Assert.True(isTransient);
        }

        [Fact]
        public void TestTransientResponseCheck()
        {
            var response = new RestResponse()
            {
                StatusCode = HttpStatusCode.GatewayTimeout
            };

            var isTransient = RetryEngine.IsTransientStatusCode(response);
            Assert.True(isTransient);
        }
    }
}
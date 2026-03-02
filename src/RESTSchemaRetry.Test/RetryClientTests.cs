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

        private RetryClient CreateClientWithMockApi(Mock<RestApi> mockApi, BackoffTypes delayType = BackoffTypes.Constant, int retryNumber = 3)
        {
            var client = new RetryClient(_baseUrl, _resource)
            {
                RetryNumber = retryNumber,
                RetryDelay = TimeSpan.Zero,
                DelayType = delayType
            };

            // Override private readonly field _restApi using reflection
            var restApiField = typeof(RetryClient).GetField("_restApi", BindingFlags.NonPublic | BindingFlags.Instance);
            restApiField?.SetValue(client, mockApi.Object);

            return client;
        }

        #region Constructor Validation

        [Fact]
        public void Constructor_DefaultValuesAreSet()
        {
            var client = new RetryClient(_baseUrl, _resource);

            Assert.Equal(3, client.RetryNumber);
            Assert.Equal(TimeSpan.FromSeconds(1), client.RetryDelay);
            Assert.Equal(BackoffTypes.ExponentialFullJitter, client.DelayType);
        }

        [Fact]
        public void Constructor_CustomRetryDelayIsSet()
        {
            int customDelay = 2000;
            var client = new RetryClient(_baseUrl, _resource, customDelay);

            Assert.Equal(TimeSpan.FromMilliseconds(customDelay), client.RetryDelay);
        }

        [Fact]
        public void Constructor_NegativeRetryDelay_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryClient(_baseUrl, _resource, -100));
        }

        [Fact]
        public void Constructor_NegativeRetryNumber_DefaultsToThree()
        {
            var client = new RetryClient(_baseUrl, _resource, -5, 1000);
            Assert.Equal(3, client.RetryNumber);
        }

        [Fact]
        public void Constructor_NegativeTimeSpanRetryDelay_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RetryClient(_baseUrl, _resource, 3, TimeSpan.FromMilliseconds(-1)));
        }

        [Fact]
        public void Constructor_NegativeTimeSpanWithBackoff_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RetryClient(_baseUrl, _resource, 3, TimeSpan.FromSeconds(-1), BackoffTypes.Constant));
        }

        [Fact]
        public void Constructor_NegativeTimeSpanWithAuth_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RetryClient(_baseUrl, _resource, 3, TimeSpan.FromSeconds(-1), "token"));
        }

        [Fact]
        public void Constructor_NegativeTimeSpanWithBackoffAndAuth_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RetryClient(_baseUrl, _resource, 3, TimeSpan.FromSeconds(-1), BackoffTypes.Constant, "token"));
        }

        [Fact]
        public void Constructor_ZeroRetryDelay_DoesNotThrow()
        {
            var client = new RetryClient(_baseUrl, _resource, 0);
            Assert.Equal(TimeSpan.Zero, client.RetryDelay);
        }

        [Fact]
        public void Constructor_CustomBackoffType_IsSet()
        {
            var client = new RetryClient(_baseUrl, _resource, BackoffTypes.Linear);
            Assert.Equal(BackoffTypes.Linear, client.DelayType);
        }

        [Fact]
        public void Constructor_InvalidBaseUrl_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new RetryClient("not-a-url", _resource));
        }

        [Fact]
        public void Constructor_EmptyResource_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new RetryClient(_baseUrl, ""));
        }

        #endregion

        #region PostAsync

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

            var result = await client.PostAsync<DummyRequest, DummyResponse>(new DummyRequest());

            Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
            mockApi.Verify(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PostAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.Accepted, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.PostAsync<DummyRequest, DummyResponse>(new DummyRequest());

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            mockApi.Verify(api => api.PostAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        #endregion

        #region GetAsync

        [Fact]
        public async Task GetAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.GetAsync<DummyResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            mockApi.Verify(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAsync_WithParams_ShouldRetryOnTransientError()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.GatewayTimeout },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.GetAsync<DummyResponse>("key", "value");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAsync_WithDictionary_ShouldRetryOnTransientError()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.InternalServerError },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.GetAsync<DummyResponse>(new Dictionary<string, string> { { "page", "1" } });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region PutAsync

        [Fact]
        public async Task PutAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.PutAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.PutAsync<DummyRequest, DummyResponse>(new DummyRequest());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            mockApi.Verify(api => api.PutAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.DeleteAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.DeleteAsync<DummyRequest, DummyResponse>(new DummyRequest());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            mockApi.Verify(api => api.DeleteAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region PatchAsync

        [Fact]
        public async Task PatchAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.GatewayTimeout },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.PatchAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.PatchAsync<DummyRequest, DummyResponse>(new DummyRequest());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            mockApi.Verify(api => api.PatchAsync<DummyRequest, DummyResponse>(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region OptionsAsync

        [Fact]
        public async Task OptionsAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.InternalServerError },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK, Data = new DummyResponse() }
            ]);

            mockApi
                .Setup(api => api.OptionsAsync<DummyResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.OptionsAsync<DummyResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region HeadAsync

        [Fact]
        public async Task HeadAsync_ShouldRetryOnTransientError_ThenSucceed()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            var responses = new Queue<RestResponse<DummyResponse>>(
            [
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.ServiceUnavailable },
                new RestResponse<DummyResponse>(new RestRequest()) { StatusCode = HttpStatusCode.OK }
            ]);

            mockApi
                .Setup(api => api.HeadAsync<DummyResponse>(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => responses.Dequeue());

            var client = CreateClientWithMockApi(mockApi);

            var response = await client.HeadAsync<DummyResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region Retry Exhaustion

        [Fact]
        public async Task RetryAsync_ExhaustsRetries_ReturnsLastTransientResponse()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<DummyResponse>(new RestRequest())
                {
                    StatusCode = HttpStatusCode.ServiceUnavailable
                });

            var client = CreateClientWithMockApi(mockApi, retryNumber: 2);

            var response = await client.GetAsync<DummyResponse>();

            // 1 initial call + 2 retries = 3 total
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            mockApi.Verify(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        #endregion

        #region NoRetry Backoff

        [Fact]
        public async Task RetryAsync_NoRetryBackoff_DoesNotRetry()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<DummyResponse>(new RestRequest())
                {
                    StatusCode = HttpStatusCode.ServiceUnavailable
                });

            var client = CreateClientWithMockApi(mockApi, delayType: BackoffTypes.NoRetry);

            var response = await client.GetAsync<DummyResponse>();

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            mockApi.Verify(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CancellationToken

        [Fact]
        public async Task RetryAsync_CancellationRequested_ThrowsOperationCanceledException()
        {
            var mockApi = new Mock<RestApi>(_baseUrl, _resource);

            mockApi
                .Setup(api => api.GetAsync<DummyResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<DummyResponse>(new RestRequest())
                {
                    StatusCode = HttpStatusCode.ServiceUnavailable
                });

            var client = CreateClientWithMockApi(mockApi, retryNumber: 10);
            // Delay > 0 so Task.Delay can observe cancellation
            client.RetryDelay = TimeSpan.FromSeconds(10);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => client.GetAsync<DummyResponse>(cts.Token));
        }

        #endregion

        #region Dispose

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var client = new RetryClient(_baseUrl, _resource);

            client.Dispose();
            client.Dispose(); // should not throw
        }

        #endregion

        #region RetryEngine (inline convenience)

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

        #endregion
    }
}
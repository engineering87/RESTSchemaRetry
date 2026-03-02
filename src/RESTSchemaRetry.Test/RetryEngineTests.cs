// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RestSharp;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace RESTSchemaRetry.Test
{
    public class RetryEngineTests
    {
        #region IsTransientStatusCode(HttpStatusCode)

        [Theory]
        [InlineData(HttpStatusCode.TooManyRequests)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.InsufficientStorage)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.HttpVersionNotSupported)]
        [InlineData(HttpStatusCode.NetworkAuthenticationRequired)]
        public void IsTransientStatusCode_TransientCodes_ReturnsTrue(HttpStatusCode code)
        {
            Assert.True(RetryEngine.IsTransientStatusCode(code));
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.NoContent)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Conflict)]
        public void IsTransientStatusCode_NonTransientCodes_ReturnsFalse(HttpStatusCode code)
        {
            Assert.False(RetryEngine.IsTransientStatusCode(code));
        }

        #endregion

        #region IsTransientStatusCode(RestResponse)

        [Fact]
        public void IsTransientStatusCode_NullResponse_ReturnsFalse()
        {
            Assert.False(RetryEngine.IsTransientStatusCode((RestResponse)null!));
        }

        [Fact]
        public void IsTransientStatusCode_TransientStatusInResponse_ReturnsTrue()
        {
            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            };

            Assert.True(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_SuccessResponse_ReturnsFalse()
        {
            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.OK
            };

            Assert.False(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_TimedOutResponse_ReturnsTrue()
        {
            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.TimedOut
            };

            Assert.True(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_HttpRequestException_ReturnsTrue()
        {
            var response = new RestResponse
            {
                StatusCode = 0,
                ResponseStatus = ResponseStatus.Error,
                ErrorException = new HttpRequestException("connection refused")
            };

            Assert.True(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_TaskCanceledException_ReturnsTrue()
        {
            var response = new RestResponse
            {
                StatusCode = 0,
                ResponseStatus = ResponseStatus.Error,
                ErrorException = new TaskCanceledException("timeout")
            };

            Assert.True(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_SocketException_ReturnsTrue()
        {
            var response = new RestResponse
            {
                StatusCode = 0,
                ResponseStatus = ResponseStatus.Error,
                ErrorException = new SocketException()
            };

            Assert.True(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_ErrorWithNonNetworkException_ReturnsFalse()
        {
            var response = new RestResponse
            {
                StatusCode = 0,
                ResponseStatus = ResponseStatus.Error,
                ErrorException = new InvalidOperationException("something else")
            };

            Assert.False(RetryEngine.IsTransientStatusCode(response));
        }

        [Fact]
        public void IsTransientStatusCode_ErrorWithNoException_ReturnsFalse()
        {
            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Error,
                ErrorException = null
            };

            Assert.False(RetryEngine.IsTransientStatusCode(response));
        }

        #endregion
    }
}

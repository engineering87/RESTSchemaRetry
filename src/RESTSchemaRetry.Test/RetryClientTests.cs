// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Enum;
using RestSharp;
using System.Net;

namespace RESTSchemaRetry.Test
{
    public class RetryClientTests
    {
        private readonly string _baseUrl = "https://api.example.com";
        private readonly string _resource = "/test";

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
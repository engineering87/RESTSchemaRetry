// (c) 2019 engineering87
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RESTSchemaRetry;
using RestSharp;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConfiguration()
        {
            var client = new RetryClient("http://example.com/", "resource");

            Assert.IsNotNull(client.RetryDelay);
            Assert.IsTrue(client.RetryNumber > 0);
        }

        [TestMethod]
        public void TestClient()
        {
            var client = new RetryClient("http://example.com/", "resource");

            var response = client.Get<object>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.StatusCode);
        }

        [TestMethod]
        public void TestTransientHttpCheck()
        {
            var retryEngine = new RetryEngine();

            var isTransient = retryEngine.IsTransient(HttpStatusCode.BadRequest);
            Assert.IsFalse(isTransient);
            isTransient = retryEngine.IsTransient(HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(isTransient);
        }

        [TestMethod]
        public void TestTransientResponseCheck()
        {
            var retryEngine = new RetryEngine();

            var response = new RestResponse()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            var isTransient = retryEngine.IsTransient(response);
            Assert.IsFalse(isTransient);
            response.StatusCode = HttpStatusCode.GatewayTimeout;
            isTransient = retryEngine.IsTransient(response);
            Assert.IsTrue(isTransient);
        }
    }
}

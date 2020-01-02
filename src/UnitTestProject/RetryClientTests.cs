// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
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
        public void TestClientGet()
        {
            var client = new RetryClient("http://example.com/", "resource");

            var response = client.Get<object>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.StatusCode);
        }

        [TestMethod]
        public void TestTransientHttpCheck()
        {
            var isTransient = RetryEngine.Instance.IsTransient(HttpStatusCode.BadRequest);
            Assert.IsFalse(isTransient);
            isTransient = RetryEngine.Instance.IsTransient(HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(isTransient);
        }

        [TestMethod]
        public void TestTransientResponseCheck()
        {
            var response = new RestResponse()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            var isTransient = RetryEngine.Instance.IsTransient(response);
            Assert.IsFalse(isTransient);
            response.StatusCode = HttpStatusCode.GatewayTimeout;
            isTransient = RetryEngine.Instance.IsTransient(response);
            Assert.IsTrue(isTransient);
        }
    }
}

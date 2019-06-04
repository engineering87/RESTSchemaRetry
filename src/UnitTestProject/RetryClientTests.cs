using Microsoft.VisualStudio.TestTools.UnitTesting;
using RESTSchemaRetry;

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
    }
}

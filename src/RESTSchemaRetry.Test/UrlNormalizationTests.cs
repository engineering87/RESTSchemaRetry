// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Utils;

namespace RESTSchemaRetry.Test
{
    public class UrlNormalizationTests
    {
        #region Normalize(baseUrl, resource)

        [Fact]
        public void Normalize_ValidInputs_ReturnsTuple()
        {
            var (baseUrl, resource) = UrlNormalization.Normalize("https://api.example.com", "/users");

            Assert.Equal("https://api.example.com", baseUrl);
            Assert.Equal("/users", resource);
        }

        [Fact]
        public void Normalize_ResourceWithoutSlash_AddsLeadingSlash()
        {
            var (_, resource) = UrlNormalization.Normalize("https://api.example.com", "users");

            Assert.Equal("/users", resource);
        }

        [Fact]
        public void Normalize_ResourceWithSlash_PreservesSlash()
        {
            var (_, resource) = UrlNormalization.Normalize("https://api.example.com", "/users");

            Assert.Equal("/users", resource);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-url")]
        [InlineData("ftp-missing-colon//host")]
        public void Normalize_InvalidBaseUrl_ThrowsArgumentException(string? invalidUrl)
        {
            Assert.Throws<ArgumentException>(() =>
                UrlNormalization.Normalize(invalidUrl!, "/resource"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Normalize_InvalidResource_ThrowsArgumentException(string? invalidResource)
        {
            Assert.Throws<ArgumentException>(() =>
                UrlNormalization.Normalize("https://api.example.com", invalidResource!));
        }

        #endregion

        #region NormalizeResource

        [Fact]
        public void NormalizeResource_WithoutSlash_AddsLeadingSlash()
        {
            Assert.Equal("/items", UrlNormalization.NormalizeResource("items"));
        }

        [Fact]
        public void NormalizeResource_WithSlash_PreservesSlash()
        {
            Assert.Equal("/items", UrlNormalization.NormalizeResource("/items"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NormalizeResource_NullOrEmpty_ThrowsArgumentException(string? invalidResource)
        {
            Assert.Throws<ArgumentException>(() =>
                UrlNormalization.NormalizeResource(invalidResource!));
        }

        #endregion
    }
}

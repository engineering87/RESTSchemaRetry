// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using RESTSchemaRetry.Utils;

namespace RESTSchemaRetry.Test
{
    public class BackoffUtilsTests
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 5)]
        [InlineData(5, 8)]
        [InlineData(6, 13)]
        [InlineData(10, 89)]
        public void GetFibonacci_ReturnsExpectedValue(int n, int expected)
        {
            Assert.Equal(expected, BackoffUtils.GetFibonacci(n));
        }

        [Fact]
        public void GetFibonacci_ZeroIndex_ReturnsNonZero()
        {
            // First retry (n=0) must produce a non-zero multiplier
            Assert.True(BackoffUtils.GetFibonacci(0) > 0);
        }

        [Fact]
        public void GetFibonacci_NegativeIndex_ReturnsOne()
        {
            // Negative values fall into the n <= 1 branch
            Assert.Equal(1, BackoffUtils.GetFibonacci(-1));
        }

        [Fact]
        public void GetFibonacci_LargeIndex_DoesNotThrow()
        {
            // Verify the iterative implementation handles large values without stack overflow
            var result = BackoffUtils.GetFibonacci(30);
            Assert.True(result > 0);
        }
    }
}

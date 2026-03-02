// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace RESTSchemaRetry.Utils
{
    internal static class BackoffUtils
    {
        /// <summary>
        /// Get the Fibonacci number (iterative, O(n) time, O(1) space).
        /// Uses 1-based Fibonacci: F(0)=1, F(1)=1, F(2)=2, F(3)=3, F(4)=5, ...
        /// This ensures the first retry (n=0) always produces a non-zero delay.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int GetFibonacci(int n)
        {
            if (n <= 1) return 1;

            int prev = 1;
            int curr = 1;
            for (int i = 2; i <= n; i++)
            {
                int next = prev + curr;
                prev = curr;
                curr = next;
            }
            return curr;
        }
    }
}

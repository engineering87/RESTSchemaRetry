// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)

namespace RESTSchemaRetry.Utils
{
    public static class BackoffUtils
    {
        /// <summary>
        /// Get the Fibonacci number
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int GetFibonacci(int n)
        {
            if (n <= 1) return n;
            return GetFibonacci(n - 1) + GetFibonacci(n - 2);
        }
    }
}

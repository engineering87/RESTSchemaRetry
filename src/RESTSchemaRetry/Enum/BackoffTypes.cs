// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace RESTSchemaRetry.Enum
{
    /// <summary>
    /// The backoff types for Schema-Retry
    /// </summary>
    public enum BackoffTypes
    {
        ///<summary>Schema-Retry implementation with constant Backoff</summary>
        Constant,
        ///<summary>Schema-Retry implementation with linear Backoff</summary>
        Linear,
        ///<summary>Schema-Retry implementation exponential Backoff</summary>
        Exponential,
        /// <summary>Schema-Retry implementation with jittered Backoff</summary>
        ExponentialWithJitter,
        /// <summary>Schema-Retry implementation with random Backoff</summary>
        Random,
        /// <summary>Schema-Retry implementation with Fibonacci Backoff</summary>
        Fibonacci,
        ///<summary>No retry pattern</summary>
        NoRetry,
        /// <summary>Schema-Retry implementation with Exponential Backoff with Full Jitter</summary>
        ExponentialFullJitter
    }
}
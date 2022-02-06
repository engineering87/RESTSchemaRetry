// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace RESTSchemaRetry.Enum
{
    /// <summary>
    /// The backoff types for Schema-Retry
    /// </summary>
    public enum BackoffTypes
    {
        ///<summary>Schema-Retry implementation with constant retry pattern</summary>
        Constant,
        ///<summary>Schema-Retry implementation with linear retry pattern</summary>
        Linear,
        ///<summary>Schema-Retry implementation exponential retry pattern</summary>
        Exponential,
        ///<summary>No retry pattern</summary>
        NoRetry
    }
}
// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace RESTSchemaRetry.Enum
{
    /// <summary>
    /// The backoff types for the retry schema
    /// </summary>
    public enum BackoffTypes
    {
        Constant,
        Linear,
        Exponential
    }
}
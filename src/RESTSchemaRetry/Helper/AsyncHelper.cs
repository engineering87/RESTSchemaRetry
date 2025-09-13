// (c) 2019-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// MIT License
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RESTSchemaRetry.Helper
{
    internal static class AsyncHelper
    {
        /// <summary>
        /// Runs a Task synchronously (no result).
        /// </summary>
        /// <param name="task">The asynchronous operation to execute.</param>
        public static void RunSync(Func<Task> task)
            => Task.Run(task).GetAwaiter().GetResult();

        /// <summary>
        /// Runs a Task synchronously and returns its result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the Task.</typeparam>
        /// <param name="task">The asynchronous operation to execute.</param>
        /// <returns>The result produced by the Task.</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
            => Task.Run(task).GetAwaiter().GetResult();

        /// <summary>
        /// Runs a Task synchronously, passing a CancellationToken.
        /// </summary>
        /// <typeparam name="TResult">The result type of the Task.</typeparam>
        /// <param name="task">The asynchronous operation to execute, accepting a CancellationToken.</param>
        /// <param name="ct">The cancellation token to observe.</param>
        /// <returns>The result produced by the Task.</returns>
        public static TResult RunSync<TResult>(Func<CancellationToken, Task<TResult>> task, CancellationToken ct)
            => Task.Run(() => task(ct), ct).GetAwaiter().GetResult();

        /// <summary>
        /// Runs a Task synchronously with an explicit timeout.
        /// Throws <see cref="TimeoutException"/> if the Task does not complete in time.
        /// </summary>
        /// <typeparam name="TResult">The result type of the Task.</typeparam>
        /// <param name="task">The asynchronous operation to execute.</param>
        /// <param name="timeout">The maximum amount of time to wait for completion.</param>
        /// <returns>The result produced by the Task.</returns>
        /// <exception cref="TimeoutException">Thrown if the Task does not complete within the specified timeout.</exception>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task, TimeSpan timeout)
        {
            var t = Task.Run(task);
            if (!t.Wait(timeout))
                throw new TimeoutException($"Operation did not complete within {timeout}.");
            return t.GetAwaiter().GetResult();
        }
    }
}
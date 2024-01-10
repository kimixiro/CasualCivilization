using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using ROZ.Logging;
using UnityEngine.Assertions;

namespace ROZ.Async
{
    public static class AsyncUtils
    {
        public static async UniTask RetryUntilComplete
        (
            [NotNull] Func<UniTask> taskFactory,
            CancellationToken token = default
        )
        {
            Assert.IsNotNull(taskFactory);
            
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await taskFactory.Invoke();

                    return;
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            }
        }
        
        public static async UniTask RetryUntilComplete
        (
            [NotNull] Func<UniTask> taskFactory,
            int delay,
            CancellationToken token = default
        )
        {
            Assert.IsNotNull(taskFactory);
            
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await taskFactory.Invoke();

                    return;
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
                try
                {
                    await UniTask.Delay(delay, cancellationToken: token);
                }
                catch
                {
                    return;
                }
            }
        }
        
        public static async UniTask<TResult> RetryUntilComplete<TResult>
        (
            [NotNull] Func<UniTask<TResult>> taskFactory,
            int delay,
            CancellationToken token = default
        )
        {
            Assert.IsNotNull(taskFactory);
            
            while (!token.IsCancellationRequested)
            {
                try
                {
                    return await taskFactory.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
                try
                {
                    await UniTask.Delay(delay, cancellationToken: token);
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }
    }
}
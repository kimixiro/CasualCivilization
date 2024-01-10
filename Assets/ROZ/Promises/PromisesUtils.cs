#if SPG_PROMISES

using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Proyecto26;
using UnityEngine.Assertions;

namespace ROZ.Promises
{
    public static class PromisesUtils
    {
        public static async UniTask<TResult> ToAsync<TResult>
        (
            [NotNull] this RSG.IPromise<ResponseHelper> source, 
            [NotNull] Func<ResponseHelper, TResult> converter,
            Action<Exception> onError
        )
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(converter);
            
            var completionSource = new UniTaskCompletionSource<TResult>();

            source
                .Then(result => completionSource.TrySetResult(converter.Invoke(result)))
                .Catch(e =>
                {
                    onError?.Invoke(e);
                    completionSource.TrySetException(e);
                });

            return await completionSource.Task;
        }
        
        public static async UniTask<TResult> ToAsync<TResult>
        (
            [NotNull] this RSG.IPromise<TResult> source, 
            Action<Exception> onError
        )
        {
            Assert.IsNotNull(source);
            
            var completionSource = new UniTaskCompletionSource<TResult>();

            source
                .Then(result => completionSource.TrySetResult(result))
                .Catch(e =>
                {
                    onError?.Invoke(e);
                    completionSource.TrySetException(e);
                });

            return await completionSource.Task;
        }
        
        public static async UniTask ToAsync
        (
            [NotNull] this RSG.IPromise source, 
            Action<Exception> onError
        )
        {
            Assert.IsNotNull(source);
            
            var completionSource = new UniTaskCompletionSource();

            source
                .Then(() => completionSource.TrySetResult())
                .Catch(e =>
                {
                    onError?.Invoke(e);
                    completionSource.TrySetException(e);
                });

            await completionSource.Task;
        }
    }
}

#endif
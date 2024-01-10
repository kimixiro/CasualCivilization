using System;
using System.Diagnostics.CodeAnalysis;
using UniRx;
using UnityEngine.Assertions;

namespace ROZ.RX
{
    public static class RXCollectionsUtils
    {
        public static IDisposable ObserveAndSubscribe<TKey, TValue>
        (
            [NotNull] this IReactiveDictionary<TKey, TValue> source,
            [NotNull] Action<DictionaryAddEvent<TKey, TValue>> onAdd,
            [NotNull] Action<DictionaryRemoveEvent<TKey, TValue>> onRemoved
        )
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemoved);

            var disposable = new CompositeDisposable();
            
            source
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposable);
            
            source
                .ObserveRemove()
                .Subscribe(onRemoved)
                .AddTo(disposable);

            return disposable;
        }
        
        public static IDisposable ObserveAndSubscribe<TKey, TValue>
        (
            [NotNull] this IReadOnlyReactiveDictionary<TKey, TValue> source,
            [NotNull] Action<DictionaryAddEvent<TKey, TValue>> onAdd,
            [NotNull] Action<DictionaryRemoveEvent<TKey, TValue>> onRemoved
        )
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemoved);

            var disposable = new CompositeDisposable();
            
            source
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposable);
            
            source
                .ObserveRemove()
                .Subscribe(onRemoved)
                .AddTo(disposable);

            return disposable;
        }
        
        public static IDisposable ObserveAndSubscribe<T>
        (
            [NotNull] this IReadOnlyReactiveCollection<T> source,
            [NotNull] Action<CollectionAddEvent<T>> onAdd,
            [NotNull] Action<CollectionRemoveEvent<T>> onRemoved
        )
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemoved);

            var disposable = new CompositeDisposable();
            
            source
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposable);
            
            source
                .ObserveRemove()
                .Subscribe(onRemoved)
                .AddTo(disposable);

            return disposable;
        }
        
        public static IDisposable ObserveAndSubscribe<T>
        (
            [NotNull] this IReactiveCollection<T> source,
            [NotNull] Action<CollectionAddEvent<T>> onAdd,
            [NotNull] Action<CollectionRemoveEvent<T>> onRemoved
        )
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemoved);

            var disposable = new CompositeDisposable();
            
            source
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposable);
            
            source
                .ObserveRemove()
                .Subscribe(onRemoved)
                .AddTo(disposable);

            return disposable;
        }
    }
}
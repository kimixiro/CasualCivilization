using System;
using System.Diagnostics.CodeAnalysis;
using UniRx;
using UnityEngine.Assertions;

namespace CasualCivilization.Global.Utils.RX
{
    public static class ReactiveCollectionExtensions
    {
        public static IDisposable ObserveAndSubscribe<TKey, TValue>
        (
            [NotNull] this IReadOnlyReactiveDictionary<TKey, TValue> dictionary,
            [NotNull] Action<DictionaryAddEvent<TKey, TValue>> onAdd,
            [NotNull] Action<DictionaryRemoveEvent<TKey, TValue>> onRemove,
            [NotNull] Action<DictionaryReplaceEvent<TKey, TValue>> onReplace
        )
        {
            Assert.IsNotNull(dictionary);
            
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemove);
            Assert.IsNotNull(onReplace);

            var disposables = new CompositeDisposable();

            dictionary
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposables);

            dictionary
                .ObserveRemove()
                .Subscribe(onRemove)
                .AddTo(disposables);
            
            dictionary
                .ObserveReplace()
                .Subscribe(onReplace)
                .AddTo(disposables);
            
            return disposables;
        }
        
        public static IDisposable ObserveAndSubscribe<TKey, TValue>
        (
            [NotNull] this IReadOnlyReactiveDictionary<TKey, TValue> dictionary,
            [NotNull] Action<DictionaryAddEvent<TKey, TValue>> onAdd,
            [NotNull] Action<DictionaryRemoveEvent<TKey, TValue>> onRemove
        )
        {
            Assert.IsNotNull(dictionary);
            
            Assert.IsNotNull(onAdd);
            Assert.IsNotNull(onRemove);

            var disposables = new CompositeDisposable();

            dictionary
                .ObserveAdd()
                .Subscribe(onAdd)
                .AddTo(disposables);

            dictionary
                .ObserveRemove()
                .Subscribe(onRemove)
                .AddTo(disposables);
            
            return disposables;
        }
    }
}
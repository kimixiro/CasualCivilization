using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ROZ.DB.Runtime.Providers;
using UnityEngine;
using UnityEngine.Assertions;

namespace ROZ.DB.Runtime
{
    public sealed class DataBase
    {
        private readonly Dictionary<Type, IDataProvider> _providers;

        private bool TryGetProvider<TTable>(out IDataProvider provider)
        {
            provider = null;
            
            var tableType = typeof(TTable);

            if (_providers.TryGetValue(tableType, out var value))
            {
                provider = value;
                
                return true;
            }
            return false;
        }

        public DataBase([NotNull] IReadOnlyList<IDataProvider> providers)
        {
            Assert.IsNotNull(providers);

            _providers = providers
                .ToDictionary(p => p.TableType);
        }
        
        public bool TryGetData<TKey, TTable, TData>(TKey key, out TData data)
            where TTable : BaseDataTable<TKey, TData>
            where TKey : BaseTableKey
            where TData : new()
        {
            var result = TryGetData<TKey, TTable, TData>(key.Guid, out var retrievedData);

            data = retrievedData;
            
            return result;
        }
        
        public bool TryGetData<TKey, TTable, TData>(string key, out TData data)
            where TTable : BaseDataTable<TKey, TData>
            where TKey : BaseTableKey
            where TData : new()
        {
            Assert.IsFalse(string.IsNullOrEmpty(key));
            
            data = default;

            if (TryGetProvider<TTable>(out var provider))
            {
                if (provider is BaseDataProvider<TTable, TKey, TData> certainProvider)
                {
                    if (certainProvider.TryGetData(key, out var retrievedData))
                    {
                        data = retrievedData;

                        return true;
                    }
                    return false;
                }
                Debug.LogWarning
                (
                    $"[WARNING] [{nameof(DataBase)}.{nameof(TryGetData)}] " + 
                    $"Wrong type of provider for table {typeof(TTable).Name}"
                );
                return false;
            }
            Debug.LogWarning
            (
                $"[WARNING] [{nameof(DataBase)}.{nameof(TryGetData)}] " + 
                $"Provider for table {typeof(TTable).Name} does does not exists"
            );
            return false;
        }
    }
}
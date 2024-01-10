using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace ROZ.DB.Runtime.Providers
{
    public abstract class BaseDataProvider<TTable, TKey, TData>
        : IDataProvider
        where TTable : BaseDataTable<TKey, TData>
        where TKey : BaseTableKey
        where TData : new()
    {
        private readonly Dictionary<TKey, TData> _keyMappedData;
        private readonly Dictionary<string, TData> _stringMappedData;
        
        public Type TableType => typeof(TTable);

        public IReadOnlyDictionary<TKey, TData> KeyMappedData => _keyMappedData;
        public IReadOnlyDictionary<string, TData> StringMappedData => _stringMappedData;

        protected BaseDataProvider([NotNull] TTable table)
        {
            Assert.IsNotNull(table);

            _keyMappedData = table.Data
                .ToDictionary(v => v.Key, v => v.Value);
            
            _stringMappedData = table.Data
                .ToDictionary(v => v.Key.Guid, v => v.Value);
        }
        
        public TData GetData(string key)
        {
            return _stringMappedData[key];
        }

        public TData GetData(TKey key)
        {
            return GetData(key.Guid);
        }

        public bool TryGetData(string key, out TData data)
        {
            data = default;
            
            if (_stringMappedData.TryGetValue(key, out var value))
            {
                data = value;
            
                return true;
            }
            return false;
        }

        public bool TryGetData(TKey key, out TData data)
        {
            return TryGetData(key.Guid, out data);
        }
    }
}
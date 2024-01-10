using System;
using System.Collections.Generic;
using UnityEngine;

namespace ROZ.DB.Runtime
{
    public abstract class BaseDataTable : ScriptableObject { }

    public abstract class BaseDataTable<TKey, TRow> 
        : BaseDataTable
        where TRow : new()
        where TKey : BaseTableKey
    {
        [Serializable]
        private class Table : SerializableDictionary<TKey, TRow> { }

        [SerializeField] 
        private Table _data = new();

        public IReadOnlyDictionary<TKey, TRow> Data => _data.ToDictionary();
    }
}
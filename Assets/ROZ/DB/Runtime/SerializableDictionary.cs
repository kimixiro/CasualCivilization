using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ROZ.DB.Runtime
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [Serializable]
        public class Pair
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<Pair> _serializableData = new();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return _serializableData
                .Where(t => t.Key != null || (t.Key is UnityEngine.Object so && so != null))
                .ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
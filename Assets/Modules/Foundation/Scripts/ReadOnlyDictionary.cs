
using System.Collections;
using System.Collections.Generic;

namespace Foundation {

    public struct ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() { return m_dict.GetEnumerator(); }
        public Dictionary<TKey, TValue>.KeyCollection Keys => m_dict.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => m_dict.Values;
        public bool ContainsKey(TKey key) { return m_dict.ContainsKey(key); }
        public bool TryGetValue(TKey key, out TValue value) { return m_dict.TryGetValue(key, out value); }
        public int Count => m_dict.Count;
        public TValue this[TKey index] => m_dict[index];
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dict) {
            m_dict = dict;
        }

        public static implicit operator ReadOnlyDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict) {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => m_dict.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => m_dict.Values;
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() { return m_dict.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_dict.GetEnumerator(); }

        private Dictionary<TKey, TValue> m_dict;
    }

}
using System.Collections;
using System.Collections.Generic;

namespace Foundation {
    public struct DictionaryValues<TKey, TValue> : IReadOnlyCollection<TValue> {
        Dictionary<TKey, TValue> dict;
        public DictionaryValues(Dictionary<TKey, TValue> dict) {
            this.dict = dict;
        }

        public int Count => dict.Count;

        public Enumerator GetEnumerator() {
            return new Enumerator(dict);
        }

        public bool TryGet(TKey key, out TValue value) => dict.TryGetValue(key, out value);
        
        public TValue[] ToArray() {
            var ret = new TValue[dict.Count];
            int index = 0;
            foreach (var kvp in dict) {
                ret[index++] = kvp.Value;
            }
            return ret;
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<TValue> {
            Dictionary<TKey, TValue>.Enumerator iter;
            public Enumerator(Dictionary<TKey, TValue> dict) {
                iter = dict.GetEnumerator();
            }

            public bool MoveNext() => iter.MoveNext();
            public TValue Current => iter.Current.Value;

            public void Dispose() => iter.Dispose();

            object IEnumerator.Current => Current;
            void IEnumerator.Reset() => throw new System.NotSupportedException();

        }

        public static implicit operator DictionaryValues<TKey, TValue>(Dictionary<TKey, TValue> dict) {
            return new DictionaryValues<TKey, TValue>(dict);
        }
    }
}
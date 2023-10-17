
using System.Collections;
using System.Collections.Generic;

namespace Foundation {
    public struct ReadOnlyHashSet<T> : IReadOnlyCollection<T> {

        public int Count => m_hash_set.Count;

        public bool Contains(T item) => m_hash_set.Contains(item);

        public HashSet<T>.Enumerator GetEnumerator() => m_hash_set.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => m_hash_set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_hash_set.GetEnumerator();

        public ReadOnlyHashSet(HashSet<T> hash_set) { m_hash_set = hash_set; }

        public static implicit operator ReadOnlyHashSet<T>(HashSet<T> hash_set) {
            return new ReadOnlyHashSet<T>(hash_set);
        }

        private HashSet<T> m_hash_set;
    }
}
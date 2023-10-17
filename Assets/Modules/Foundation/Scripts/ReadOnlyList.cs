
using System.Collections;
using System.Collections.Generic;

namespace Foundation {
    public struct ReadOnlyList<T> : IReadOnlyList<T> {

        public List<T>.Enumerator GetEnumerator() { return m_list.GetEnumerator(); }
        public int Count => m_list.Count;
        public T this[int index] => m_list[index];
        public T[] to_array() => m_list.ToArray();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }

        public ReadOnlyList(List<T> list) { m_list = list; }

        public static implicit operator ReadOnlyList<T>(List<T> list) {
            return new ReadOnlyList<T>(list);
        }

        private List<T> m_list;
    }
}
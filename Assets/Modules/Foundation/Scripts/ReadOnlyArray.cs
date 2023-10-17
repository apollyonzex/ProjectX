
using System.Collections;
using System.Collections.Generic;

namespace Foundation {
    public struct ReadOnlyArray<T> : IReadOnlyList<T> {
        public int Count => m_array.Length;
        public T this[int index] => m_array[index];
        public struct Enumerator : IEnumerator<T> {
            public Enumerator(T[] array) {
                m_array = array;
                m_index = -1;
            }
            public bool MoveNext() {
                return ++m_index < m_array.Length;
            }
            public T Current => m_array[m_index];
            public void Reset() {
                m_index = -1;
            }
            object IEnumerator.Current => Current;
            void System.IDisposable.Dispose() {}
            T[] m_array;
            int m_index;
        }
        public Enumerator GetEnumerator() { return new Enumerator(m_array); }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public ReadOnlyArray(T[] array) { m_array = array; }
        public static implicit operator ReadOnlyArray<T>(T[] array) {
            return new ReadOnlyArray<T>(array);
        }
        private T[] m_array;
    }

}
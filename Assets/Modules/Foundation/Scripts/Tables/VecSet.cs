

using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Tables {

    public class VecSet<TKey> : IEnumerable<TKey> where TKey : IComparable<TKey> {

        public VecSet() {
            m_items = new TKey[0];
        }

        public int length => m_items.Length;

        public ref readonly TKey this[int index] => ref m_items[index];

        public bool contains(TKey key) {
            int low = 0, high = m_items.Length - 1;
            while (low <= high) {
                var mid = (low + high) / 2;
                var r = key.CompareTo(m_items[mid]);
                if (r == 0) {
                    return true;
                }
                if (r < 0) {
                    high = mid - 1;
                } else {
                    low = mid + 1;
                }
            }
            return false;
        }

        public static VecSet<TKey> new_unchecked(TKey[] sorted_items) {
            return new VecSet<TKey>(sorted_items);
        }

        public struct Enumerator : IEnumerator<TKey> {
            private TKey[] items;
            private int index;

            internal Enumerator(TKey[] items) {
                this.items = items;
                index = -1;
            }

            public ref readonly TKey Current => ref items[index];

            TKey IEnumerator<TKey>.Current => Current;

            object IEnumerator.Current => Current;

            public bool MoveNext() {
                ++index;
                if (index < items.Length) {
                    return true;
                }
                return false;
            }

            void IDisposable.Dispose() {
                
            }

            void IEnumerator.Reset() {
                index = -1;
            }
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(m_items);
        }

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private VecSet(TKey[] sorted_items) {
            m_items = sorted_items;
        }

        private TKey[] m_items;
    }
}

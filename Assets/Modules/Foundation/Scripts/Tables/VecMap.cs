

using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Tables {
    public class VecMap<TKey, TValue> : IEnumerable<(TKey key, TValue value)> where TKey: IComparable<TKey> {

        public VecMap() {
            m_items = new (TKey key, TValue value)[0];
        }

        public static VecMap<TKey, TValue> new_unchecked((TKey key, TValue value)[] sorted_items) {
            return new VecMap<TKey, TValue>(sorted_items);
        }

        public int length => m_items.Length;

        public ref readonly (TKey key, TValue value) this[int index] => ref m_items[index];

        public bool try_get_value(TKey key, out TValue value) {
            int low = 0, high = m_items.Length - 1;
            while (low <= high) {
                var mid = (low + high) / 2;
                ref var item = ref m_items[mid];
                var r = key.CompareTo(item.key);
                if (r == 0) {
                    value = item.value;
                    return true;
                }
                if (r < 0) {
                    high = mid - 1;
                } else {
                    low = mid + 1;
                }
            }
            value = default;
            return false;
        }

        public struct Enumerator : IEnumerator<(TKey key, TValue value)> {
            private (TKey key, TValue value)[] items;
            private int index;

            internal Enumerator((TKey key, TValue value)[] items) {
                this.items = items;
                index = -1;
            }

            public ref readonly (TKey key, TValue value) Current => ref items[index];

            (TKey key, TValue value) IEnumerator<(TKey key, TValue value)>.Current => Current;

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

        IEnumerator<(TKey key, TValue value)> IEnumerable<(TKey key, TValue value)>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private VecMap((TKey key, TValue value)[] sorted_items) {
            m_items = sorted_items;
        }

        private (TKey key, TValue value)[] m_items;
    }
}


using System.Collections.Generic;

namespace Foundation {

    public abstract class ArraySlice {
        public static ArraySlice<T> create<T>(T[] array) {
            return new ArraySlice<T>(array);
        }

        public static ArraySlice<T> create<T>(T[] array, int offset, int length) {
            return new ArraySlice<T>(array, offset, length);
        }
    }

    public struct ArraySlice<T> {
        public ArraySlice(T[] array) {
            m_array = array;
            m_offset = 0;
            m_length = array.Length;
        }

        public ArraySlice(T[] array, int offset, int length) {
            m_array = array;
            m_offset = offset;
            m_length = length;
        }

        public ref T this[int index] => ref m_array[index + m_offset];
        public int length => m_length;

        public struct ArraySliceEnumerator {
            ArraySlice<T> slice;
            int index;
            int end;

            internal ArraySliceEnumerator(ArraySlice<T> slice) {
                this.slice = slice;
                index = slice.m_offset - 1;
                end = slice.m_offset + slice.m_length;
            }

            public bool MoveNext() {
                return ++index < end;
            }

            public ref T Current => ref slice.m_array[index];
        }

        public ArraySliceEnumerator GetEnumerator() => new ArraySliceEnumerator(this);

        private T[] m_array;
        private int m_offset, m_length;
    }

}
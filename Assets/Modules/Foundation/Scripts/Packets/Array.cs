
using System.IO;

namespace Foundation.Packets {

    public struct Array<T> : IPacketData where T : struct, IPacketData {
        public T[] items;

        public Array(T[] v) {
            items = v;
        }

        public Array(int size) {
            items = new T[size];
        }

        public ArraySlice<T>.ArraySliceEnumerator GetEnumerator() {
            return new ArraySlice<T>.ArraySliceEnumerator(ArraySlice.create(items));
        }

        public static implicit operator T[](Array<T> v) {
            return v.items;
        }

        public static implicit operator Array<T>(T[] v) {
            return new Array<T>(v);
        }

        public void save_to(BinaryWriter w) {
            foreach (ref var e in this) {
                e.save_to(w);
            }
        }

        public bool validate(int[] size_hint, int offset) {
            if (items == null) {
                return false;
            }
            if (items.Length != size_hint[offset]) {
                return false;
            }
            ++offset;
            foreach (ref var e in this) {
                if (!e.validate(size_hint, offset)) {
                    return false;
                }
            }
            return true;
        }
    }

}
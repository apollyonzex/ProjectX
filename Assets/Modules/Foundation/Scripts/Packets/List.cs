
using System.IO;


namespace Foundation.Packets {

    public struct List<T> : IPacketData where T : struct, IPacketData {

        public T[] items;

        public List(T[] v) {
            items = v;
        }

        public List(BinaryReader r) {
            items = new T[r.read_compressed_uint()];
        }

        public ArraySlice<T>.ArraySliceEnumerator GetEnumerator() {
            return new ArraySlice<T>.ArraySliceEnumerator(ArraySlice.create(items));
        }

        public static implicit operator T[](List<T> v) {
            return v.items;
        }

        public static implicit operator List<T>(T[] v) {
            return new List<T>(v);
        }

        public void save_to(BinaryWriter w) {
            w.write_compressed_uint((ulong)items.Length);
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
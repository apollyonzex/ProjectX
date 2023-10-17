
using System.IO;

namespace Foundation.Packets {

    public struct Nullable<T> : IPacketData where T : struct, IPacketData {
        public bool has_value;
        public T value;

        public override int GetHashCode() {
            return has_value ? value.GetHashCode() : 0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null)) {
                return !has_value;
            }
            if (!has_value) {
                return false;
            }
            if (obj is T v) {
                return v.Equals(obj);
            }
            if (obj is Nullable<T> other) {
                if (!has_value && !other.has_value) {
                    return true;
                }
                if (has_value && other.has_value) {
                    return value.Equals(other.value);
                }
            }
            return false;
        }

        public static implicit operator Nullable<T>(T v) {
            return new Nullable<T> {
                has_value = true,
                value = v,
            };
        }

        public void save_to(BinaryWriter w) {
            if (has_value) {
                value.save_to(w);
            }
        }

        public bool validate(int[] size_hint, int offset) {
            return !has_value || value.validate(size_hint, offset);
        }
    }

}
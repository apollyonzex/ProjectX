
using System;

namespace Foundation.Tables {

    public struct EnumI8<T> : IComparable<EnumI8<T>> where T : Enum, IConvertible {

        public T value;

        public EnumI8(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumI8<T> self) => self.value;
        public static implicit operator EnumI8<T>(T value) => new EnumI8<T>(value);

        public int CompareTo(EnumI8<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToSByte(ci).CompareTo(other.value.ToSByte(ci));
        }
    }

    public struct EnumI16<T> : IComparable<EnumI16<T>> where T : Enum, IConvertible {

        public T value;

        public EnumI16(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumI16<T> self) => self.value;
        public static implicit operator EnumI16<T>(T value) => new EnumI16<T>(value);

        public int CompareTo(EnumI16<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToInt16(ci).CompareTo(other.value.ToInt16(ci));
        }
    }

    public struct EnumI32<T> : IComparable<EnumI32<T>> where T : Enum, IConvertible {

        public T value;

        public EnumI32(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumI32<T> self) => self.value;
        public static implicit operator EnumI32<T>(T value) => new EnumI32<T>(value);

        public int CompareTo(EnumI32<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToInt32(ci).CompareTo(other.value.ToInt32(ci));
        }
    }

    public struct EnumI64<T> : IComparable<EnumI64<T>> where T : Enum, IConvertible {

        public T value;

        public EnumI64(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumI64<T> self) => self.value;
        public static implicit operator EnumI64<T>(T value) => new EnumI64<T>(value);

        public int CompareTo(EnumI64<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToInt64(ci).CompareTo(other.value.ToInt64(ci));
        }
    }

    public struct EnumU8<T> : IComparable<EnumU8<T>> where T : Enum, IConvertible {

        public T value;

        public EnumU8(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumU8<T> self) => self.value;
        public static implicit operator EnumU8<T>(T value) => new EnumU8<T>(value);

        public int CompareTo(EnumU8<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToByte(ci).CompareTo(other.value.ToByte(ci));
        }
    }

    public struct EnumU16<T> : IComparable<EnumU16<T>> where T : Enum, IConvertible {

        public T value;

        public EnumU16(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumU16<T> self) => self.value;
        public static implicit operator EnumU16<T>(T value) => new EnumU16<T>(value);

        public int CompareTo(EnumU16<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToUInt16(ci).CompareTo(other.value.ToUInt16(ci));
        }
    }

    public struct EnumU32<T> : IComparable<EnumU32<T>> where T : Enum, IConvertible {

        public T value;

        public EnumU32(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumU32<T> self) => self.value;
        public static implicit operator EnumU32<T>(T value) => new EnumU32<T>(value);

        public int CompareTo(EnumU32<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToUInt32(ci).CompareTo(other.value.ToUInt32(ci));
        }
    }

    public struct EnumU64<T> : IComparable<EnumU64<T>> where T : Enum, IConvertible {

        public T value;

        public EnumU64(T value) {
            this.value = value;
        }

        public static implicit operator T(EnumU64<T> self) => self.value;
        public static implicit operator EnumU64<T>(T value) => new EnumU64<T>(value);

        public int CompareTo(EnumU64<T> other) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return value.ToUInt64(ci).CompareTo(other.value.ToUInt64(ci));
        }
    }
}
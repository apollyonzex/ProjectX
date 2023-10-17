
using System.IO;
using System.Text;

namespace Foundation.Packets {

    public interface IEnumBackType : IPacketData {
        void from_enum(System.Enum obj);
    }

    public struct u8 : IPacketData, IEnumBackType {
        public byte value;

        public u8(byte v) {
            value = v;
        }

        public u8(BinaryReader r) {
            value = r.ReadByte();
        }

        public static implicit operator u8(byte v) {
            return new u8(v);
        }

        public static implicit operator byte(u8 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToByte(obj);
        }
    }

    public struct i8 : IPacketData, IEnumBackType {
        public sbyte value;

        public i8(sbyte v) {
            value = v;
        }

        public i8(BinaryReader r) {
            value = r.ReadSByte();
        }

        public static implicit operator i8(sbyte v) {
            return new i8(v);
        }

        public static implicit operator sbyte(i8 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToSByte(obj);
        }
    }

    public struct u16 : IPacketData, IEnumBackType {
        public ushort value;

        public u16(ushort v) {
            value = v;
        }

        public u16(BinaryReader r) {
            value = r.ReadUInt16();
        }

        public static implicit operator u16(ushort v) {
            return new u16(v);
        }

        public static implicit operator ushort(u16 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToUInt16(obj);
        }
    }

    public struct i16 : IPacketData, IEnumBackType {
        public short value;

        public i16(short v) {
            value = v;
        }

        public i16(BinaryReader r) {
            value = r.ReadInt16();
        }

        public static implicit operator i16(short v) {
            return new i16(v);
        }

        public static implicit operator short(i16 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToInt16(obj);
        }
    }

    public struct u32 : IPacketData, IEnumBackType {
        public uint value;

        public u32(uint v) {
            value = v;
        }

        public u32(BinaryReader r) {
            value = r.ReadUInt32();
        }

        public static implicit operator u32(uint v) {
            return new u32(v);
        }

        public static implicit operator uint(u32 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToUInt32(obj);
        }
    }

    public struct i32 : IPacketData, IEnumBackType {
        public int value;

        public i32(int v) {
            value = v;
        }

        public i32(BinaryReader r) {
            value = r.ReadInt32();
        }

        public static implicit operator i32(int v) {
            return new i32(v);
        }

        public static implicit operator int(i32 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToInt32(obj);
        }
    }

    public struct u64 : IPacketData, IEnumBackType {
        public ulong value;

        public u64(ulong v) {
            value = v;
        }

        public u64(BinaryReader r) {
            value = r.ReadUInt64();
        }

        public static implicit operator u64(ulong v) {
            return new u64(v);
        }

        public static implicit operator ulong(u64 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToUInt64(obj);
        }
    }

    public struct i64 : IPacketData, IEnumBackType {
        public long value;

        public i64(long v) {
            value = v;
        }

        public i64(BinaryReader r) {
            value = r.ReadInt64();
        }

        public static implicit operator i64(long v) {
            return new i64(v);
        }

        public static implicit operator long(i64 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToInt64(obj);
        }
    }

    public struct cuint : IPacketData, IEnumBackType {
        public ulong value;

        public cuint(ulong v) {
            value = v;
        }

        public cuint(BinaryReader r) {
            value = r.read_compressed_uint();
        }

        public static implicit operator cuint(ulong v) {
            return new cuint(v);
        }

        public static implicit operator ulong(cuint v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.write_compressed_uint(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToUInt64(obj);
        }
    }

    public struct cint : IPacketData, IEnumBackType {
        public long value;

        public cint(long v) {
            value = v;
        }

        public cint(BinaryReader r) {
            value = r.read_compressed_int();
        }

        public static implicit operator cint(long v) {
            return new cint(v);
        }

        public static implicit operator long(cint v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.write_compressed_int(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }

        public void from_enum(System.Enum obj) {
            value = System.Convert.ToInt64(obj);
        }
    }

    public struct f32 : IPacketData {
        public float value;

        public f32(float v) {
            value = v;
        }

        public f32(BinaryReader r) {
            value = r.ReadSingle();
        }

        public static implicit operator f32(float v) {
            return new f32(v);
        }

        public static implicit operator float(f32 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }
    }
    
    public struct f64 : IPacketData {
        public double value;

        public f64(double v) {
            value = v;
        }

        public f64(BinaryReader r) {
            value = r.ReadDouble();
        }

        public static implicit operator f64(double v) {
            return new f64(v);
        }

        public static implicit operator double(f64 v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }
    }

    public struct Bool : IPacketData {
        public bool value;

        public Bool(bool v) {
            value = v;
        }

        public Bool(BinaryReader r) {
            value = r.ReadBoolean();
        }

        public static implicit operator Bool(bool v) {
            return new Bool(v);
        }

        public static implicit operator bool(Bool v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.Write(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return true;
        }
    }

    public struct String : IPacketData {
        public string value;

        public String(string v) {
            value = v;
        }

        public String(BinaryReader r) {
            value = r.read_string();
        }

        public static implicit operator String(string v) {
            return new String(v);
        }

        public static implicit operator string(String v) {
            return v.value;
        }

        public void save_to(BinaryWriter w) {
            w.write_string(value);
        }

        public bool validate(int[] size_hint, int offset) {
            return value == null || Encoding.UTF8.GetByteCount(value) <= size_hint[offset];
        }
    }

    public struct Enum<TE, TB> : IPacketData where TE : System.Enum where TB : struct, IEnumBackType {
        public TE value;

        public Enum(TE v) {
            value = v;
        }

        public static implicit operator TE(Enum<TE, TB> v) {
            return v.value;
        }

        public static implicit operator Enum<TE, TB>(TE v) {
            return new Enum<TE, TB>(v);
        }

        public void save_to(BinaryWriter w) {
            TB back = default;
            back.from_enum(value);
            back.save_to(w);
        }

        public bool validate(int[] size_hint, int offset) {
            return System.Enum.IsDefined(typeof(TE), value);
        }
    }
}
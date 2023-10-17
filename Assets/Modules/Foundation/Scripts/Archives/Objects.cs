
using System.IO;
using System.Collections.Generic;
using Foundation.Packets;

namespace Foundation.Archives {

    internal interface IArchive {
        byte id { get; }
        void save(BinaryWriter writer);
        void load(BinaryReader reader);
    }

    internal interface IArchiveContainer {
        void add(IArchive value);
        void add(uint index, IArchive value);
        bool try_get(out IArchive value);
        bool try_get(uint index, out IArchive value);
    }

    internal struct Null : IArchive {

        public const byte id = 1;
        byte IArchive.id => id;
        public void load(BinaryReader reader) {

        }
        public void save(BinaryWriter writer) {

        }
    }

    internal struct Bool : IArchive {
        public const byte id = 2;
        byte IArchive.id => id;
        public bool value;
        public void load(BinaryReader reader) {
            value = reader.ReadBoolean();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
    }

    internal interface INumber : IArchive {
        byte as_u8 { get; }
        sbyte as_i8 { get; }
        ushort as_u16 { get; }
        short as_i16 { get; }
        uint as_u32 { get; }
        int as_i32 { get; }
        ulong as_u64 { get; }
        long as_i64 { get; }
        float as_f32 { get; }
        double as_f64 { get; }
    }

    internal struct PositiveByte : INumber {
        public const byte id = 3;
        byte IArchive.id => id;
        public PositiveByte(byte value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadByte();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => value;
        public short as_i16 => value;
        public uint as_u32 => value;
        public int as_i32 => value;
        public ulong as_u64 => value;
        public long as_i64 => value;
        public float as_f32 => value;
        public double as_f64 => value;

        private byte value;
    }

    internal struct NegativeByte : INumber {
        public const byte id = 4;
        byte IArchive.id => id;
        public NegativeByte(byte value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadByte();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)-value;
        public sbyte as_i8 => (sbyte)-value;
        public ushort as_u16 => (ushort)-value;
        public short as_i16 => (short)-value;
        public uint as_u32 => (uint)-value;
        public int as_i32 => -value;
        public ulong as_u64 => (ulong)-value;
        public long as_i64 => -value;
        public float as_f32 => -value;
        public double as_f64 => -value;

        private byte value;
    }

    internal struct PositiveUShort : INumber {
        public const byte id = 5;
        byte IArchive.id => id;
        public PositiveUShort(ushort value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadUInt16();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => value;
        public short as_i16 => (short)value;
        public uint as_u32 => value;
        public int as_i32 => value;
        public ulong as_u64 => value;
        public long as_i64 => value;
        public float as_f32 => value;
        public double as_f64 => value;

        private ushort value;
    }

    internal struct NegativeUShort : INumber {
        public const byte id = 6;
        byte IArchive.id => id;
        public NegativeUShort(ushort value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadUInt16();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)-value;
        public sbyte as_i8 => (sbyte)-value;
        public ushort as_u16 => (ushort)-value;
        public short as_i16 => (short)-value;
        public uint as_u32 => (uint)-value;
        public int as_i32 => -value;
        public ulong as_u64 => (ulong)-value;
        public long as_i64 => -value;
        public float as_f32 => -value;
        public double as_f64 => -value;

        private ushort value;
    }

    internal struct PositiveUInt : INumber {
        public const byte id = 7;
        byte IArchive.id => id;
        public PositiveUInt(uint value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadUInt32();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => (ushort)value;
        public short as_i16 => (short)value;
        public uint as_u32 => value;
        public int as_i32 => (int)value;
        public ulong as_u64 => value;
        public long as_i64 => value;
        public float as_f32 => value;
        public double as_f64 => value;

        private uint value;
    }

    internal struct NegativeUInt : INumber {
        public const byte id = 8;
        byte IArchive.id => id;
        public NegativeUInt(uint value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = reader.ReadUInt32();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)-value;
        public sbyte as_i8 => (sbyte)-value;
        public ushort as_u16 => (ushort)-value;
        public short as_i16 => (short)-value;
        public uint as_u32 => (uint)-value;
        public int as_i32 => (int)-value;
        public ulong as_u64 => (ulong)-value;
        public long as_i64 => -value;
        public float as_f32 => -value;
        public double as_f64 => -value;

        private uint value;
    }

    internal struct Long : INumber {
        public const byte id = 9;
        byte IArchive.id => id;
        public void load(BinaryReader reader) {
            value = reader.ReadInt64();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public long value;
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => (ushort)value;
        public short as_i16 => (short)value;
        public uint as_u32 => (uint)value;
        public int as_i32 => (int)value;
        public ulong as_u64 => (ulong)value;
        public long as_i64 => value;
        public float as_f32 => value;
        public double as_f64 => value;
    }

    internal struct ULong : INumber {
        public const byte id = 10;
        byte IArchive.id => id;
        public void load(BinaryReader reader) {
            value = reader.ReadUInt64();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public ulong value;
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => (ushort)value;
        public short as_i16 => (short)value;
        public uint as_u32 => (uint)value;
        public int as_i32 => (int)value;
        public ulong as_u64 => value;
        public long as_i64 => (long)value;
        public float as_f32 => value;
        public double as_f64 => value;
    }

    internal struct Float : INumber {
        public const byte id = 11;
        byte IArchive.id => id;
        public float value;
        public void load(BinaryReader reader) {
            value = reader.ReadSingle();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => (ushort)value;
        public short as_i16 => (short)value;
        public uint as_u32 => (uint)value;
        public int as_i32 => (int)value;
        public ulong as_u64 => (ulong)value;
        public long as_i64 => (long)value;
        public float as_f32 => value;
        public double as_f64 => value;
    }

    internal struct Double : INumber {
        public const byte id = 12;
        byte IArchive.id => id;
        public double value;
        public void load(BinaryReader reader) {
            value = reader.ReadDouble();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value);
        }
        public byte as_u8 => (byte)value;
        public sbyte as_i8 => (sbyte)value;
        public ushort as_u16 => (ushort)value;
        public short as_i16 => (short)value;
        public uint as_u32 => (uint)value;
        public int as_i32 => (int)value;
        public ulong as_u64 => (ulong)value;
        public long as_i64 => (long)value;
        public float as_f32 => (float)value;
        public double as_f64 => value;
    }

    internal struct StringIndex : IArchive {
        public const byte id = 13;
        byte IArchive.id => id;
        public uint value;
        public StringIndex(uint value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = (uint)reader.read_compressed_uint();
        }

        public void save(BinaryWriter writer) {
            writer.write_compressed_uint(value);
        }
    }

    internal struct ObjectIndex : IArchive {
        public const byte id = 14;
        byte IArchive.id => id;
        public uint value;
        public ObjectIndex(uint value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = (uint)reader.read_compressed_uint();
        }

        public void save(BinaryWriter writer) {
            writer.write_compressed_uint(value);
        }
    }

    internal class Vector : System.Collections.Generic.List<IArchive>, IArchive, IArchiveContainer {
        public const byte id = 15;
        byte IArchive.id => id;
        public void load(BinaryReader reader) {
            var count = (int)reader.read_compressed_uint();
            Capacity = count;
            for (int i = 0; i < count; ++i) {
                var value = Factory.create(reader.ReadByte());
                value.load(reader);
                Add(value);
            }
        }
        public void save(BinaryWriter writer) {
            writer.write_compressed_uint((ulong)Count);
            foreach (var value in this) {
                writer.Write(value.id);
                value.save(writer);
            }
        }
        void IArchiveContainer.add(IArchive value) {
            Add(value);
        }
        void IArchiveContainer.add(uint index, IArchive value) {
            throw new System.NotSupportedException("Not Struct");
        }
        bool IArchiveContainer.try_get(out IArchive value) {
            if (m_index < Count) {
                value = this[m_index++];
                return true;
            }
            value = null;
            return false;
        }
        bool IArchiveContainer.try_get(uint index, out IArchive value) {
            throw new System.NotSupportedException("Not Struct");
        }
        public void reset_index() {
            m_index = 0;
        }
        private int m_index = 0;
    }

    internal class Struct : Dictionary<uint, IArchive>, IArchive, IArchiveContainer {
        public const byte id = 16;
        byte IArchive.id => id;
        public void load(BinaryReader reader) {
            var count = (int)reader.read_compressed_uint();
            for (int i = 0; i < count; ++i) {
                var idx = (uint)reader.read_compressed_uint();
                var value = Factory.create(reader.ReadByte());
                value.load(reader);
                Add(idx, value);
            }
        }
        public void save(BinaryWriter writer) {
            writer.write_compressed_uint((ulong)Count);
            foreach (var kvp in this) {
                writer.write_compressed_uint(kvp.Key);
                var value = kvp.Value;
                writer.Write(value.id);
                value.save(writer);
            }
        }
        public static void save_empty(BinaryWriter writer) {
            writer.write_compressed_uint(0);
        }
        void IArchiveContainer.add(IArchive value) {
            throw new System.NotSupportedException("Not Array");
        }
        void IArchiveContainer.add(uint idx, IArchive value) {
            Add(idx, value);
        }
        bool IArchiveContainer.try_get(out IArchive value) {
            throw new System.NotSupportedException("Not Array");
        }
        bool IArchiveContainer.try_get(uint index, out IArchive value) {
            return TryGetValue(index, out value);
        }
    }

    internal interface IVec2 : IArchive {
        UnityEngine.Vector2 as_vec2f { get; }
        UnityEngine.Vector2Int as_vec2i { get; }
    }

    internal struct Vec2 : IVec2 {
        public const byte id = 17;
        byte IArchive.id => id;
        public UnityEngine.Vector2 value;
        public void load(BinaryReader reader) {
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
        }
        public void save(BinaryWriter writer) {
            writer.Write(value.x);
            writer.Write(value.y);
        }
        UnityEngine.Vector2 IVec2.as_vec2f => value;
        UnityEngine.Vector2Int IVec2.as_vec2i => new UnityEngine.Vector2Int((int)value.x, (int)value.y);
    }

    internal struct Vec2Int : IVec2 {
        public const byte id = 18;
        byte IArchive.id => id;
        public UnityEngine.Vector2Int value;
        public void load(BinaryReader reader) {
            value.x = (int)reader.read_compressed_int();
            value.y = (int)reader.read_compressed_int();
        }
        public void save(BinaryWriter writer) {
            writer.write_compressed_int(value.x);
            writer.write_compressed_int(value.y);
        }
        UnityEngine.Vector2 IVec2.as_vec2f => value;
        UnityEngine.Vector2Int IVec2.as_vec2i => value;
    }

    internal struct TypeIndex : IArchive {
        public const byte id = 19;
        byte IArchive.id => id;
        public uint value;
        public TypeIndex(uint value) {
            this.value = value;
        }
        public void load(BinaryReader reader) {
            value = (uint)reader.read_compressed_uint();
        }

        public void save(BinaryWriter writer) {
            writer.write_compressed_uint(value);
        }
    }

    internal struct Bytes : IArchive {
        public const byte id = 20;

        byte IArchive.id => id;

        public byte[] bytes;

        public void load(BinaryReader reader) {
            var length = reader.read_compressed_uint();
            bytes = reader.ReadBytes((int)length);
        }

        public void save(BinaryWriter writer) {
            writer.write_compressed_uint((ulong)bytes.Length);
            writer.Write(bytes);
        }
    }

    internal static class Factory {
        public static IArchive create(byte id) {
            switch (id) {
                case Null.id:
                    return new Null();
                case Bool.id:
                    return new Bool();
                case PositiveByte.id:
                    return new PositiveByte();
                case NegativeByte.id:
                    return new NegativeByte();
                case PositiveUShort.id:
                    return new PositiveUShort();
                case NegativeUShort.id:
                    return new NegativeUShort();
                case PositiveUInt.id:
                    return new PositiveUInt();
                case NegativeUInt.id:
                    return new NegativeUInt();
                case ULong.id:
                    return new ULong();
                case Long.id:
                    return new Long();
                case Float.id:
                    return new Float();
                case Double.id:
                    return new Double();
                case StringIndex.id:
                    return new StringIndex();
                case ObjectIndex.id:
                    return new ObjectIndex();
                case Vector.id:
                    return new Vector();
                case Struct.id:
                    return new Struct();
                case Vec2.id:
                    return new Vec2();
                case Vec2Int.id:
                    return new Vec2Int();
                case TypeIndex.id:
                    return new TypeIndex();
                case Bytes.id:
                    return new Bytes();
                default:
                    throw new InvalidDataException();
            }
        }

        public static INumber create_number(ulong value) {
            if (value <= 0xFF) {
                return new PositiveByte((byte)value);
            }
            if (value <= 0xFFFF) {
                return new PositiveUShort((ushort)value);
            }
            if (value <= 0xFFFFFFFF) {
                return new PositiveUInt((uint)value);
            }
            return new ULong() { value = value };
        }

        public static INumber create_number(long value) {
            if (value >= 0) {
                return create_number((ulong)value);
            }
            if (value >= -0xFF) {
                return new NegativeByte((byte)-value);
            }
            if (value >= -0xFFFF) {
                return new NegativeUShort((ushort)-value);
            }
            if (value >= -0xFFFFFFFF) {
                return new NegativeUInt((uint)-value);
            }
            return new Long() { value = value };
        }
    }
}
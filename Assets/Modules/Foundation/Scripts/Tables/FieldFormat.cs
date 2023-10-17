

using System.IO;

namespace Foundation.Tables {

    public abstract class DataType {

        public abstract void skip_by_seek(BinaryReader reader);
        public abstract void skip_by_read(BinaryReader reader);

        public abstract bool check(int[] seq, ref int idx);

        public const int EMPTY = 0;
        public const int INT_1 = 1;
        public const int INT_1_B = 2;
        public const int INT_1_U = 3;
        public const int INT_1_I = 4;
        public const int INT_2 = 5;
        public const int INT_2_U = 6;
        public const int INT_2_I = 7;
        public const int INT_4 = 8;
        public const int INT_4_U = 9;
        public const int INT_4_I = 10; 
        public const int INT_8 = 11;
        public const int INT_8_U = 12;
        public const int INT_8_I = 13;
        public const int FLOAT = 14;
        public const int DOUBLE = 15;
        public const int STRING = 16;
        public const int TUPLE = 17;
        public const int ARRAY = 18;
        public const int DICT_1 = 19;
        public const int DICT_2 = 20;
        public const int JSON_RAW = 21;
        public const int JSON_BIN = 22;
        public const int EXPR_TREE = 23;

        public static DataType load_key(BinaryReader reader, EnumDef[] enum_defs) {
            switch (reader.ReadByte()) {
                case 1: return new BasicBool();
                case 2: return new BasicU8();
                case 3: return new BasicI8();
                case 4: return new BasicU16();
                case 5: return new BasicI16();
                case 6: return new BasicU32();
                case 7: return new BasicI32();
                case 8: return new BasicU64();
                case 9: return new BasicI64();
                case 10: return new BasicF32();
                case 11: return new BasicF64();
                case 12: return new BasicString();
                case 13: return new EnumType(enum_defs[(int)CompressedInt.decompress_uint(reader)]);
                case 14: {
                    var items = new DataType[(int)reader.ReadByte()];
                    foreach (ref var item in ArraySlice.create(items)) {
                        item = load_basic_type(reader.ReadByte(), reader, enum_defs);
                    }
                    return new TupleType(items);
                }
                default: throw new InvalidDataException("invalid key type");
            }
        }

        public static (DataType dt, bool optional) load_non_key(BinaryReader reader, EnumDef[] enum_defs) {
            var lead = reader.ReadByte();
            return (load_data_type(lead & 0x1F, reader, enum_defs), (lead & 0xE0) == 0x20);
        }

        private static DataType load_basic_type(int lead, BinaryReader reader, EnumDef[] enum_defs) {
            switch (lead) {
                case 1: return new BasicBool();
                case 2: return new BasicU8();
                case 3: return new BasicI8();
                case 4: return new BasicU16();
                case 5: return new BasicI16();
                case 6: return new BasicU32();
                case 7: return new BasicI32();
                case 8: return new BasicU64();
                case 9: return new BasicI64();
                case 10: return new BasicF32();
                case 11: return new BasicF64();
                case 12: return new BasicString();
                case 13: return new EnumType(enum_defs[(int)CompressedInt.decompress_uint(reader)]);
                default: throw new InvalidDataException("invalid basic type");
            }
        }

        private static DataType load_data_type(int lead, BinaryReader reader, EnumDef[] enum_defs) {
            switch (lead) {
                case 0: return new EmptyType();
                case 1: return new BasicBool();
                case 2: return new BasicU8();
                case 3: return new BasicI8();
                case 4: return new BasicU16();
                case 5: return new BasicI16();
                case 6: return new BasicU32();
                case 7: return new BasicI32();
                case 8: return new BasicU64();
                case 9: return new BasicI64();
                case 10: return new BasicF32();
                case 11: return new BasicF64();
                case 12: return new BasicString();
                case 13: return new EnumType(enum_defs[(int)CompressedInt.decompress_uint(reader)]);
                case 14: {
                    var items = new DataType[reader.ReadByte()];
                    foreach (ref var item in ArraySlice.create(items)) {
                        item = load_data_type(reader.ReadByte(), reader, enum_defs);
                    }
                    return new TupleType(items);
                }
                case 15: return new ArrayType(load_data_type(reader.ReadByte(), reader, enum_defs));
                case 16: return new SetType(load_key(reader, enum_defs));
                case 17: {
                    var key = load_key(reader, enum_defs);
                    var value = load_data_type(reader.ReadByte(), reader, enum_defs);
                    return new DictType(key, value);
                }
                case 18: return new JsonRaw();
                case 19: return new JsonBin();
                case 20: return new ExprTreeType();

                default: throw new InvalidDataException("invalid data type");
            }
        }
    }

    public class BasicBool : DataType {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(1, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadByte();
        }

        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_1_B || val == INT_1;
        }
    }

    public class BasicU8 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(1, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadByte();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_1_U || val == INT_1;
        }
    }

    public class BasicI8 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(1, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadByte();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_1_I || val == INT_1;
        }
    }

    public class BasicU16 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(2, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt16();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_2_U || val == INT_2;
        }
    }

    public class BasicI16 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(2, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt16();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_2_I || val == INT_2;
        }
    }

    public class BasicU32 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(4, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt32();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_4_U || val == INT_4;
        }
    }

    public class BasicI32 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(4, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt32();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_4_I || val == INT_4;
        }
    }

    public class BasicU64 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(8, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt64();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_8_U || val == INT_8;
        }
    }

    public class BasicI64 : DataType, IEnumBackEnd {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(8, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt64();
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == INT_8_I || val == INT_8;
        }
    }

    public class BasicF32 : DataType {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(4, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt32();
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == FLOAT;
        }
    }

    public class BasicF64 : DataType {
        public override void skip_by_seek(BinaryReader reader) { 
            reader.BaseStream.Seek(8, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            reader.ReadUInt64();
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == DOUBLE;
        }
    } 

    public class BasicString : DataType {
        public override void skip_by_seek(BinaryReader reader) { 
            CompressedInt.decompress_uint(reader);
        }
        public override void skip_by_read(BinaryReader reader) {
            CompressedInt.decompress_uint(reader);
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == STRING;
        }
    }

    public class EnumType : DataType {
        public EnumDef type { get; }
        public EnumType(EnumDef type) { this.type = type; }
        public override void skip_by_seek(BinaryReader reader) { 
            type.back_end.skip_by_seek(reader);
        }
        public override void skip_by_read(BinaryReader reader) {
            type.back_end.skip_by_read(reader);
        }

        public override bool check(int[] seq, ref int idx) {
            return type.back_end.check(seq, ref idx);
        }
    }

    public class TupleType : DataType {
        public DataType[] items { get; }
        public TupleType(DataType[] items) {
            this.items = items;
        }

        public override void skip_by_seek(BinaryReader reader) { 
            foreach (var item in items) {
                item.skip_by_seek(reader);
            }
        }
        public override void skip_by_read(BinaryReader reader) {
            foreach (var item in items) {
                item.skip_by_read(reader);
            }
        }
        public override bool check(int[] seq, ref int idx) {
            if (seq[idx++] == TUPLE) {
                if (seq[idx++] == items.Length) {
                    foreach (var item in items) {
                        if (!item.check(seq, ref idx)) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }

    public class ArrayType : DataType {
        public DataType element { get; }
        public ArrayType(DataType element) { this.element = element; }
        public override void skip_by_seek(BinaryReader reader) { 
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                element.skip_by_seek(reader);
            }
        }
        public override void skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                element.skip_by_read(reader);
            }
        }
        public override bool check(int[] seq, ref int idx) {
            if (seq[idx++] == ARRAY) {
                return element.check(seq, ref idx);
            }
            return false;
        }
    }

    public class SetType : DataType {
        public DataType element { get; }
        public SetType(DataType element) { this.element = element; }
        public override void skip_by_seek(BinaryReader reader) { 
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                element.skip_by_seek(reader);
            }
        }
        public override void skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                element.skip_by_read(reader);
            }
        }
        public override bool check(int[] seq, ref int idx) {
            if (seq[idx++] == DICT_1) {
                return element.check(seq, ref idx);
            }
            return false;
        }
    }

    public class DictType : DataType {
        public DataType key { get; }
        public DataType value { get; }
        public DictType(DataType key, DataType value) { this.key = key; this.value = value; }
        public override void skip_by_seek(BinaryReader reader) { 
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                key.skip_by_seek(reader);
                value.skip_by_seek(reader);
            }
        }
        public override void skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                key.skip_by_read(reader);
                value.skip_by_read(reader);
            }
        }
        public override bool check(int[] seq, ref int idx) {
            if (seq[idx++] == DICT_2) {
                if (!key.check(seq, ref idx)) {
                    return false;
                }
                return value.check(seq, ref idx);
            }
            return false;
        }
    }

    public class JsonBin : DataType {
        public override void skip_by_seek(BinaryReader reader) {
            IJsonValue obj = new JsonObject();
            obj.skip_by_seek(reader);
        }
        public override void skip_by_read(BinaryReader reader) {
            IJsonValue obj = new JsonObject();
            obj.skip_by_read(reader);
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == JSON_BIN;
        }
    }

    public class JsonRaw : DataType {
        public override void skip_by_seek(BinaryReader reader) { 
            var len = (int)CompressedInt.decompress_uint(reader);
            reader.BaseStream.Seek(len, SeekOrigin.Current);
        }
        public override void skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            for (int i = 0; i < len; ++i) {
                reader.ReadByte();
            }
        }
        public override bool check(int[] seq, ref int idx) {
            var val = seq[idx++];
            return val == JSON_RAW || val == STRING;
        }
    }

    public class EmptyType : DataType {
        public override void skip_by_seek(BinaryReader reader) { 

        }
        public override void skip_by_read(BinaryReader reader) {
            
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == EMPTY;
        }
    }

    public class ExprTreeType : DataType {
        public override void skip_by_seek(BinaryReader reader) {
            ExprTree.skip_by_read(reader);
        }
        public override void skip_by_read(BinaryReader reader) {
            ExprTree.skip_by_read(reader);
        }
        public override bool check(int[] seq, ref int idx) {
            return seq[idx++] == EXPR_TREE;
        }
    }
}

using System;
using System.IO;


namespace Foundation.Tables {

    public static class FieldReader {

        public static Action<T, BinaryReader, byte[], string[]> create_skip_reader_by_seek<T>(IFieldInfo fi) {
            var optional_index = fi.optional_index;
            var dt = fi.type;
            if (optional_index.HasValue) {
                var index = optional_index.Value;      
                return (r, s, d, _) => {
                    if (is_non_null(d, index)) {
                        dt.skip_by_seek(s);
                    }
                };
            }
            return (r, s, d, _) => dt.skip_by_seek(s);
        }

        public static Action<T, BinaryReader, byte[], string[]> create_skip_reader_by_read<T>(IFieldInfo fi) {
            var optional_index = fi.optional_index;
            var dt = fi.type;
            if (optional_index.HasValue) {
                var index = optional_index.Value;
                return (r, s, d, _) => {
                    if (is_non_null(d, index)) {
                        dt.skip_by_read(s);
                    }
                };
            }
            return (r, s, d, _) => dt.skip_by_read(s);
        }

        public static bool is_non_null(byte[] data, int index) {
            int a = index / 8;
            int b = index % 8;
            return (data[a] & (1 << b)) == 0;
        }

        public static string read_string(BinaryReader s) {
            var len = (int)CompressedInt.decompress_uint(s);
            if (len > 0) {
                return System.Text.Encoding.UTF8.GetString(s.ReadBytes(len));
            }
            return string.Empty;
        }

        public static bool read_bool(BinaryReader s, string[] _) {
            var val = s.ReadByte();
            return val != 0;
        }

        public static byte read_u8(BinaryReader s, string[] _) {
            return s.ReadByte();
        }

        public static sbyte read_i8(BinaryReader s, string[] _) {
            return s.ReadSByte();
        }

        public static ushort read_u16(BinaryReader s, string[] _) {
            return s.ReadUInt16();
        }

        public static short read_i16(BinaryReader s, string[] _) {
            return s.ReadInt16();
        }

        public static uint read_u32(BinaryReader s, string[] _) {
            return s.ReadUInt32();
        }

        public static int read_i32(BinaryReader s, string[] _) {
            return s.ReadInt32();
        }

        public static ulong read_u64(BinaryReader s, string[] _) {
            return s.ReadUInt64();
        }

        public static long read_i64(BinaryReader s, string[] _) {
            return s.ReadInt64();
        }

        public static float read_f32(BinaryReader s, string[] _) {
            return s.ReadSingle();
        }

        public static double read_f64(BinaryReader s, string[] _) {
            return s.ReadDouble();
        }

        public static string read_indexed_string(BinaryReader s, string[] strings) {
            var idx = (int)CompressedInt.decompress_uint(s);
            if (idx == 0) {
                return string.Empty;
            }
            return strings[idx - 1];
        }

        public static JsonObject read_json(BinaryReader s, string[] strings) {
            var obj = new JsonObject();
            ((IJsonValue)obj).load(s, strings);
            return obj;
        }

        public static IExprTree read_expr_tree(BinaryReader s, string[] strings) {
            return ExprTree.load(s, strings);
        }

        public static bool read_empty(BinaryReader s, string[] _) { return true; }

        public static T[] read_array<T>(BinaryReader s, string[] ss, Func<BinaryReader, string[], T> f) {
            var len = (int)CompressedInt.decompress_uint(s);
            var ret = new T[len];
            foreach (ref var item in ArraySlice.create(ret)) {
                item = f(s, ss);
            }
            return ret;
        }

        public static VecSet<T> read_set<T>(BinaryReader s, string[] ss, Func<BinaryReader, string[], T> f) where T : IComparable<T> {
            return VecSet<T>.new_unchecked(read_array(s, ss, f));
        }

        public static VecMap<TKey, TValue> read_dict<TKey, TValue>(BinaryReader s, string[] ss, Func<BinaryReader, string[], TKey> f1, Func<BinaryReader, string[], TValue> f2) where TKey : IComparable<TKey> {
            var len = (int)CompressedInt.decompress_uint(s);
            var ret = new (TKey key, TValue value)[len];
            foreach (ref var item in ArraySlice.create(ret)) {
                item = (f1(s, ss), f2(s, ss));
            }
            return VecMap<TKey, TValue>.new_unchecked(ret);
        }
    }
}

using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace Foundation.Packets {

    public static class Extend {

        public static void write_compressed_uint(this BinaryWriter w, ulong v) {
            CompressedInt.compress_uint(w, v);
        }

        public static void write_compressed_int(this BinaryWriter w, long v) {
            CompressedInt.compress_int(w, v);
        }

        public static void write_string(this BinaryWriter w, string v) {
            if (string.IsNullOrEmpty(v)) {
                w.Write((byte)0);
            } else {
                var bytes = Encoding.UTF8.GetBytes(v);
                w.write_compressed_uint((ulong)bytes.Length);
                w.Write(bytes);
            }
        }

        public static ulong read_compressed_uint(this BinaryReader r) {
            return CompressedInt.decompress_uint(r);
        }

        public static long read_compressed_int(this BinaryReader r) {
            return CompressedInt.decompress_int(r);
        }

        public static string read_string(this BinaryReader r) {
            var len = (int)CompressedInt.decompress_uint(r);
            if (len == 0) {
                return string.Empty;
            }
            var bytes = r.ReadBytes(len);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
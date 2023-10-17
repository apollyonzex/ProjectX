
using System.IO;

namespace Foundation.Tables {

    public struct Header {
        public EnumDef[] enum_defs;
        public RecordFormat format;
        public int string_count;
        public int record_count;

        public static Header load(BinaryReader reader) {
            var sign = reader.ReadBytes(4);
            if (sign[0] != 'T' || sign[1] != 'B' || sign[2] != 'L' || sign[3] != 0) {
                throw new InvalidDataException("invalid sign");
            }
            var enum_defs = new EnumDef[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var def in ArraySlice.create(enum_defs)) {
                def = EnumDef.load(reader);
            }
            var format = RecordFormat.load(reader, enum_defs);
            var string_count = (int)CompressedInt.decompress_uint(reader);
            var record_count = (int)CompressedInt.decompress_uint(reader);
            return new Header {
                enum_defs = enum_defs,
                format = format,
                string_count = string_count,
                record_count = record_count,
            };
        }

    }

}
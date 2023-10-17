
using System.IO;

namespace Foundation.Tables {

    public interface IEnumBackEnd {
        void skip_by_seek(BinaryReader reader);
        void skip_by_read(BinaryReader reader);
        bool check(int[] seq, ref int idx);
    }

    public class EnumDef {
        public string name { get; }
        public IEnumBackEnd back_end { get; }
        public (string, long)[] items { get; }

        public EnumDef(string name, IEnumBackEnd back_end, (string, long)[] items) {
            this.name = name;
            this.back_end = back_end;
            this.items = items;
        }

        public static EnumDef load(BinaryReader reader) {
            var name = FieldReader.read_string(reader);
            IEnumBackEnd back_end;
            switch (reader.ReadByte()) {
                case 1: back_end = new BasicU8(); break;
                case 2: back_end = new BasicI8(); break;
                case 3: back_end = new BasicU16(); break;
                case 4: back_end = new BasicI16(); break;
                case 5: back_end = new BasicU32(); break;
                case 6: back_end = new BasicI32(); break;
                case 7: back_end = new BasicU64(); break;
                case 8: back_end = new BasicI64(); break;
                default: throw new InvalidDataException("invalid enum back end");
            }
            var items = new (string name, long value)[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var item in ArraySlice.create(items)) {
                item.value = CompressedInt.decompress_int(reader);
                item.name = FieldReader.read_string(reader);
            }
            return new EnumDef(name, back_end, items);
        }
    }

}
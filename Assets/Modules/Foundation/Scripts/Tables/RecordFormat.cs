
using System.Collections.Generic;
using System.IO;

namespace Foundation.Tables {

    public interface IFieldInfo {
        string name { get; }
        DataType type { get; }
        int? optional_index { get; }
    }

    public class KeyInfo : IFieldInfo {
        public string name { get; }
        public DataType type { get; }
        int? IFieldInfo.optional_index => null;

        public KeyInfo(string name, DataType type) {
            this.name = name;
            this.type = type;
        }

        public static KeyInfo load(BinaryReader reader, EnumDef[] enum_defs) {
            var name = FieldReader.read_string(reader);
            var type = DataType.load_key(reader, enum_defs);
            return new KeyInfo(name, type);
        }
    }

    public class NonKeyInfo : IFieldInfo {
        public string name { get; }
        public DataType type { get; }
        public int? optional_index { get; }
        
        public NonKeyInfo(string name, DataType type) {
            this.name = name;
            this.type = type;
            optional_index = null;
        }
        public NonKeyInfo(string name, DataType type, int optional_index) {
            this.name = name;
            this.type = type;
            this.optional_index = optional_index;
        }

        public static NonKeyInfo load(BinaryReader reader, EnumDef[] enum_defs, ref int optional_count) {
            var name = FieldReader.read_string(reader);
            var info = DataType.load_non_key(reader, enum_defs);
            if (info.optional) {
                return new NonKeyInfo(name, info.dt, optional_count++);
            }
            return new NonKeyInfo(name, info.dt);
        }
    }

    public class GroupInfo {
        public string name { get; }
        public int optional_index { get; }
        public NonKeyInfo[] members { get; }

        public GroupInfo(string name, int optional_index, NonKeyInfo[] members) {
            this.name = name;
            this.optional_index = optional_index;
            this.members = members;
        }

        public static GroupInfo load(BinaryReader reader, EnumDef[] enum_defs, ref int optional_count) {
            var name = FieldReader.read_string(reader);
            var optional_index = optional_count++;
            var members = new NonKeyInfo[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var member in ArraySlice.create(members)) {
                member = NonKeyInfo.load(reader, enum_defs, ref optional_count);
            }
            return new GroupInfo(name, optional_index, members);
        }

        public int index_of_member(string name, int[] seq, out int? optional_index) {
            int idx;
            optional_index = null;
            for (int i = 0; i < members.Length; ++i) {
                var member = members[i];
                if (member.name.Equals(name, System.StringComparison.Ordinal)) {
                    idx = 0;
                    if (member.type.check(seq, ref idx)) {
                        optional_index = member.optional_index;
                        return i;
                    }
                    return -1;
                }
            }
            return -1;
        }
    }

    public class RecordFormat {

        public static RecordFormat load(BinaryReader reader, EnumDef[] enum_defs) {
            int optional_count = 0;
            var keys = new KeyInfo[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var key in ArraySlice.create(keys)) {
                key = KeyInfo.load(reader, enum_defs);
            }
            var non_keys = new NonKeyInfo[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var non_key in ArraySlice.create(non_keys)) {
                non_key = NonKeyInfo.load(reader, enum_defs, ref optional_count);
            }
            var groups = new GroupInfo[(int)CompressedInt.decompress_uint(reader)];
            foreach (ref var group in ArraySlice.create(groups)) {
                group = GroupInfo.load(reader, enum_defs, ref optional_count);
            }
            int optional_data_len = optional_count / 8;
            if (optional_count % 8 != 0) {
                ++optional_data_len;
            }
            return new RecordFormat(keys, non_keys, groups, optional_data_len);
        }

        public RecordFormat(KeyInfo[] keys, NonKeyInfo[] non_keys, GroupInfo[] groups, int optional_data_len) {
            this.keys = keys;
            this.non_keys = non_keys;
            this.groups = groups;
            this.optional_data_len = optional_data_len;
        }

        public int optional_data_len { get; }

        public int index_of_key(string name, int[] seq, int offset) {
            int idx;
            while (offset < keys.Length) {
                var key = keys[offset];
                if (key.name.Equals(name, System.StringComparison.Ordinal)) {
                    idx = 0;
                    if (key.type.check(seq, ref idx)) {
                        return offset;
                    }
                    break;
                }
                ++offset;
            }
            return -1;
        }

        public int index_of_non_key(string name, int[] seq, out int? optional_index) {
            int idx;
            optional_index = null;
            for (int i = 0; i < keys.Length; ++i) {
                var key = keys[i];
                if (key.name.Equals(name, System.StringComparison.Ordinal)) {
                    idx = 0;
                    if (key.type.check(seq, ref idx)) {
                        return i;
                    }
                    return -1;
                }
            }
            for (int i = 0; i < non_keys.Length; ++i) {
                var non_key = non_keys[i];
                if (non_key.name.Equals(name, System.StringComparison.Ordinal)) {
                    idx = 0;
                    if (non_key.type.check(seq, ref idx)) {
                        optional_index = non_key.optional_index;
                        return i;
                    }
                    return -1;
                }
            }
            return -1;
        }

        public int index_of_group(string name) {
            for (int i = 0; i < groups.Length; ++i) {
                var group = groups[i];
                if (group.name.Equals(name, System.StringComparison.Ordinal)) {
                    return i;
                }
            }
            return -1;
        }

        public KeyInfo[] keys { get; }
        public NonKeyInfo[] non_keys { get; }
        public GroupInfo[] groups { get; }
    }

}

using System.IO;

namespace Foundation.Tables {

    public class RecordReader<T> {

        public class Group {
            public System.Action<T> init { get; set; }
            public System.Action<T, BinaryReader, byte[], string[]>[] fields { get; }

            private int m_optional_index;

            public Group(GroupInfo info) {
                init = null;
                fields = new System.Action<T, BinaryReader, byte[], string[]>[info.members.Length];
                m_optional_index = info.optional_index;
            }

            public void validate(GroupInfo info) {
                for (int i = 0; i < fields.Length; ++i) {
                    ref var f = ref fields[i];
                    if (f == null) {
                        f = FieldReader.create_skip_reader_by_seek<T>(info.members[i]);
                    }
                }
            }

            public void read(T r, BinaryReader s, byte[] d, string[] ss) {
                if (FieldReader.is_non_null(d, m_optional_index)) {
                    init?.Invoke(r);
                    foreach (var f in fields) {
                        f(r, s, d, ss);
                    }
                }
            }
        }

        public System.Action<T, BinaryReader, byte[], string[]>[] keys { get; }
        public System.Action<T, BinaryReader, byte[], string[]>[] non_keys { get; }

        public System.Action<T> init { get; set; }
        public Group[] groups { get; }

        public RecordReader(RecordFormat fmt) {
            keys = new System.Action<T, BinaryReader, byte[], string[]>[fmt.keys.Length];
            non_keys = new System.Action<T, BinaryReader, byte[], string[]>[fmt.non_keys.Length];
            groups = new Group[fmt.groups.Length];
            for (int i = 0; i < groups.Length; ++i) {
                groups[i] = new Group(fmt.groups[i]);
            }
        }

        public void validate(RecordFormat fmt) {
            for (int i = 0; i < keys.Length; ++i) {
                ref var f = ref keys[i];
                if (f == null) {
                    f = FieldReader.create_skip_reader_by_seek<T>(fmt.keys[i]);
                }
            }
            for (int i = 0; i < non_keys.Length; ++i) {
                ref var f = ref non_keys[i];
                if (f == null) {
                    f = FieldReader.create_skip_reader_by_seek<T>(fmt.non_keys[i]);
                }
            }
            for (int i = 0; i < groups.Length; ++i) {
                groups[i].validate(fmt.groups[i]);
            }
        }

        public void read(T r, BinaryReader s, int option_data_len, string[] strings) {
            init?.Invoke(r);
            foreach (var f in keys) {
                f(r, s, null, strings);
            }
            var data = s.ReadBytes(option_data_len);
            foreach (var f in non_keys) {
                f(r, s, data, strings);
            }
            foreach (var g in groups) {
                g.read(r, s, data, strings);
            }
        }

    }

}

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Foundation.Packets;

namespace Foundation.Archives {
    public class ArchiveReader {

        public class LoadingJob : LoadingState.IJob {

            public string name { get; }
            
            public LoadingJob(string name, BinaryReader reader, System.Action<ArchiveReader> callback, int objects_per_frame = 1) {
                this.name = name;
                m_reader = reader;
                m_callback = callback;
                m_objects_per_frame = objects_per_frame > 1 ? objects_per_frame : 1;
            }
            

            IEnumerator LoadingState.IJob.start() {
                var ret = new ArchiveReader();

                var type_count = (int)m_reader.read_compressed_uint();
                ret.m_types = new System.Type[type_count];
                for (int i = 0; i < type_count; ++i) {
                    ret.m_types[i] = System.Type.GetType(m_reader.read_string(), false);
                }
                m_process = 0.1f;
                yield return null;

                

                var string_count = (int)m_reader.read_compressed_uint();
                ret.m_strings = new string[string_count];
                ret.m_string_indices = new Dictionary<string, uint>(string_count);
                for (int i = 0; i < string_count; ++i) {
                    var s = m_reader.read_string();
                    ret.m_strings[i] = s;
                    ret.m_string_indices.Add(s, (uint)i);
                }

                m_process = 0.2f;
                yield return null;

                var methods = new (bool init, MethodBase value)[type_count];

                var object_count = (int)m_reader.read_compressed_uint();
                ret.m_objects = new IArchiver[object_count];
                var object_data = new Struct[object_count];
                var unarchive_params = new object[] { ret };
                int loaded_count = 0;
                for (int i = 0; i < object_count; ++i) {
                    var idx = (int)m_reader.read_compressed_uint();

                    var data = new Struct();
                    object_data[i] = data;
                    data.load(m_reader);

                    var method = methods[idx];
                    if (!method.init) {
                        method.init = true;
                        var type = ret.m_types[idx];
                        if (type != null) {
                            method.value = type.GetConstructor(System.Type.EmptyTypes);
                            if (method.value == null) {
                                method.value = type.GetMethod("unarchive_instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null, s_unarchive_instance_parameter_types, null);
                            }
                        }
                        methods[idx] = method;
                    }

                    if (method.value != null) {
                        if (method.value is ConstructorInfo ctor) {
                            ret.m_objects[i] = ctor.Invoke(null) as IArchiver;
                        } else {
                            ret.m_stack.Push(data);
                            ret.m_objects[i] = method.value.Invoke(null, unarchive_params) as IArchiver;
                            ret.m_stack.Clear();
                        }
                    }
                    if (++loaded_count == m_objects_per_frame) {
                        loaded_count = 0;
                        m_process = 0.2f + (i + 1) * 0.4f / object_count;
                        yield return null;
                    }
                }

                
                m_process = 0.6f;
                yield return null;

                

                ret.m_data.load(m_reader);

                loaded_count = 0;
                for (int i = 0; i < object_count; ++i) {
                    var obj = ret.m_objects[i];
                    if (obj != null) {
                        ret.m_stack.Push(object_data[i]);
                        obj.unarchive(ret);
                        ret.m_stack.Clear();
                    }
                    if (++loaded_count == m_objects_per_frame) {
                        loaded_count = 0;
                        m_process = 0.6f + (i + 1) * 0.4f / object_count;
                        yield return null;
                    }
                }
                ret.m_stack.Push(ret.m_data);
                m_callback.Invoke(ret);
            }

            float LoadingState.IJob.progress => m_process;

            private BinaryReader m_reader;
            private float m_process = 0;
            private System.Action<ArchiveReader> m_callback;
            private int m_objects_per_frame;
        }

        public struct StructScope : System.IDisposable {
            internal StructScope(ArchiveReader reader) {
                m_reader = reader;
            }

            void System.IDisposable.Dispose() {
                m_reader.end_struct_or_array();
            }

            private ArchiveReader m_reader;
        }

        public struct ArrayScope : System.IDisposable {
            internal ArrayScope(ArchiveReader reader, int count) {
                m_reader = reader;
                m_count = count;
            }

            void System.IDisposable.Dispose() {
                m_reader.end_struct_or_array();
            }

            public int count => m_count;

            private ArchiveReader m_reader;
            private int m_count;

            public Enumerator GetEnumerator() {
                return new Enumerator(m_count);
            }

            public struct Enumerator {
                internal Enumerator(int count) {
                    m_index = -1;
                    m_count = count;
                }

                public int Current => m_index;
                public bool MoveNext() {
                    return ++m_index < m_count;
                }

                private int m_index;
                private int m_count;
            }
        }

        public ArchiveReader(BinaryReader reader) {
            var type_count = (int)reader.read_compressed_uint();
            
            m_types = new System.Type[type_count];
            foreach (ref var type in ArraySlice.create(m_types)) {
                type = System.Type.GetType(reader.read_string(), false);
            }

            var string_count = (int)reader.read_compressed_uint();
            m_strings = new string[string_count];
            m_string_indices = new Dictionary<string, uint>(string_count);
            for (int i = 0; i < string_count; ++i) {
                var s = reader.read_string();
                m_strings[i] = s;
                m_string_indices.Add(s, (uint)i);
            }

            var methods = new (bool init, MethodBase value)[type_count];

            var object_count = (int)reader.read_compressed_uint();
            m_objects = new IArchiver[object_count];
            var object_data = new Struct[object_count];
            var unarchive_params = new object[] { this };
            for (int i = 0; i < object_count; ++i) {
                var idx = (int)reader.read_compressed_uint();
                
                ref var data = ref object_data[i];
                data = new Struct();
                data.load(reader);

                ref var method = ref methods[idx];
                if (!method.init) {
                    method.init = true;
                    var type = m_types[idx];
                    if (type != null) {
                        method.value = type.GetConstructor(System.Type.EmptyTypes);
                        if (method.value == null) {
                            method.value = type.GetMethod("unarchive_instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null, s_unarchive_instance_parameter_types, null);
                        }
                    }
                }
                if (method.value != null) {
                    if (method.value.IsConstructor) {
                        m_objects[i] = method.value.Invoke(null, null) as IArchiver;
                    } else {
                        m_stack.Push(data);
                        m_objects[i] = method.value.Invoke(null, unarchive_params) as IArchiver;
                        m_stack.Clear();
                    }
                }
            }
            m_data.load(reader);

            for (int i = 0; i < object_count; ++i) {
                var obj = m_objects[i];
                if (obj != null) {
                    m_stack.Push(object_data[i]);
                    obj.unarchive(this);
                    m_stack.Clear();
                }
            }
            m_stack.Push(m_data);
        }

        private ArchiveReader() {

        }

        public bool try_begin_struct(string name = null) {
            if (try_get_class(name, out Struct value)) {
                m_stack.Push(value);
                return true;
            }
            return false;
        }

        public void end_struct_or_array() {
            m_stack.Pop();
        }

        public bool try_struct_scope(out StructScope scope, string name = null) {
            if (try_get_class(name, out Struct value)) {
                m_stack.Push(value);
                scope = new StructScope(this);
                return true;
            }
            scope = default;
            return false;
        }

        public bool try_struct(string name, System.Action block) {
            if (try_get_class(name, out Struct value)) {
                m_stack.Push(value);
                block?.Invoke();
                m_stack.Pop();
                return true;
            }
            return false;
        }

        public bool try_begin_array(out int count, string name = null) {
            if (try_get_class(name, out Vector value)) {
                m_stack.Push(value);
                value.reset_index();
                count = value.Count;
                return true;
            }
            count = default;
            return false;
        }

        public bool try_array_scope(out ArrayScope scope, string name = null) {
            if (try_get_class(name, out Vector value)) {
                m_stack.Push(value);
                value.reset_index();
                scope = new ArrayScope(this, value.Count);
                return true;
            }
            scope = default;
            return false;
        }

        public bool try_array(string name, System.Action<int> block) {
            if (try_get_class(name, out Vector value)) {
                m_stack.Push(value);
                value.reset_index();
                block?.Invoke(value.Count);
                m_stack.Pop();
                return true;
            }
            return false;
        }

        public bool try_read(ref byte value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_u8;
                return true;
            }
            return false;
        }

        public bool try_read(ref sbyte value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_i8;
                return true;
            }
            return false;
        }

        public bool try_read(ref ushort value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_u16;
                return true;
            }
            return false;
        }

        public bool try_read(ref short value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_i16;
                return true;
            }
            return false;
        }

        public bool try_read(ref uint value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_u32;
                return true;
            }
            return false;
        }

        public bool try_read(ref int value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_i32;
                return true;
            }
            return false;
        }

        public bool try_read(ref ulong value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_u64;
                return true;
            }
            return false;
        }

        public bool try_read(ref long value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_i64;
                return true;
            }
            return false;
        }

        public bool try_read(ref float value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_f32;
                return true;
            }
            return false;
        }

        public bool try_read(ref double value, string name = null) {
            if (try_get_class(name, out INumber number)) {
                value = number.as_f64;
                return true;
            }
            return false;
        }

        public bool try_read(ref string value, string name = null) {
            if (try_get_struct(name, out StringIndex obj)) {
                value = m_strings[(int)obj.value];
                return true;
            }
            return false;
        }

        public bool try_read(ref bool value, string name = null) {
            if (try_get_struct(name, out Bool obj)) {
                value = obj.value;
                return true;
            }
            return false;
        }

        public bool try_read(ref UnityEngine.Vector2 value, string name = null) {
            if (try_get_class(name, out IVec2 obj)) {
                value = obj.as_vec2f;
                return true;
            }
            return false;
        }

        public bool try_read(ref UnityEngine.Vector2Int value, string name = null) {
            if (try_get_class(name, out IVec2 obj)) {
                value = obj.as_vec2i;
                return true;
            }
            return false;
        }

        public bool try_read(ref System.Type value, string name = null) {
            if (!try_get_class(name, out IArchive obj)) {
                return false;
            }
            if (obj is TypeIndex val) {
                value = m_types[val.value];
                return true;
            }
            if (obj is Null) {
                value = null;
                return true;
            }
            return false;
        }

        public bool try_read(ref byte[] value, string name = null) {
            if (!try_get_class(name, out IArchive obj)) {
                return false;
            }
            if (obj is Bytes val) {
                value = val.bytes;
                return true;
            }
            if (obj is Null) {
                value = null;
                return true;
            }
            return false;
        }

        public bool try_read<T>(ref T value, string name = null) where T : class, IArchiver {
            if (!try_get_class(name, out IArchive obj)) {
                return false;
            }
            if (obj is ObjectIndex val) {
                if (m_objects[(int)val.value] is T t) {
                    value = t;
                    return true;
                }
                return false;
            }
            if (obj is Null) {
                value = null;
                return true;
            }
            return false;
        }

        public byte read_byte(string name = null, byte default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public sbyte read_sbyte(string name = null, sbyte default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public ushort read_ushort(string name = null, ushort default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public short read_short(string name = null, short default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public uint read_uint(string name = null, uint default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public int read_int(string name = null, int default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public ulong read_ulong(string name = null, ulong default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public long read_long(string name = null, long default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public string read_string(string name = null, string default_value = null) {
            try_read(ref default_value, name);
            return default_value;
        }

        public float read_float(string name = null, float default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public double read_double(string name = null, double default_value = 0) {
            try_read(ref default_value, name);
            return default_value;
        }

        public T read_object<T>(string name = null, T default_value = null) where T: class, IArchiver {
            try_read(ref default_value, name);
            return default_value;
        }

        public bool read_bool(string name = null, bool default_value = false) {
            try_read(ref default_value, name);
            return default_value;
        }

        public UnityEngine.Vector2 read_vec2(string name = null, UnityEngine.Vector2 default_value = default) {
            try_read(ref default_value, name);
            return default_value;
        }

        public UnityEngine.Vector2Int read_vec2i(string name = null, UnityEngine.Vector2Int default_value = default) {
            try_read(ref default_value, name);
            return default_value;
        }

        public System.Type read_type(string name = null, System.Type default_value = null) {
            try_read(ref default_value, name);
            return default_value;
        }

        public byte[] read_bytes(string name = null, byte[] default_value = null) {
            try_read(ref default_value, name);
            return default_value;
        }

        private bool try_get_class<T>(string name, out T value) where T : class, IArchive {
            var container = m_stack.Peek();
            IArchive obj;
            if (name == null) {
                if (!container.try_get(out obj)) {
                    value = default;
                    return false;
                }
            } else {
                if (!m_string_indices.TryGetValue(name, out var index)) {
                    value = default;
                    return false;
                }
                if (!container.try_get(index, out obj)) {
                    value = default;
                    return false;
                }
            }
            if (obj is T val) {
                value = val;
                return true;
            }
            value = default;
            return false;
        }

        private bool try_get_struct<T>(string name, out T value) where T : struct, IArchive {
            var container = m_stack.Peek();
            IArchive obj;
            if (name == null) {
                if (!container.try_get(out obj)) {
                    value = default;
                    return false;
                }
            } else {
                if (!m_string_indices.TryGetValue(name, out var index)) {
                    value = default;
                    return false;
                }
                if (!container.try_get(index, out obj)) {
                    value = default;
                    return false;
                }
            }
            if (obj is T val) {
                value = val;
                return true;
            }
            value = default;
            return false;
        }


        Dictionary<string, uint> m_string_indices;
        string[] m_strings;

        System.Type[] m_types;

        IArchiver[] m_objects;
        Stack<IArchiveContainer> m_stack = new Stack<IArchiveContainer>();
        Struct m_data = new Struct();

        static readonly System.Type[] s_unarchive_instance_parameter_types = new System.Type[] { typeof(ArchiveReader) };
    }
}

using System.IO;
using System.Collections.Generic;
using Foundation.Packets;

namespace Foundation.Archives {

    public class ArchiveWriter {
        public struct StructScope : System.IDisposable {

            internal StructScope(ArchiveWriter writer, string name = null) {
                m_writer = writer;
                m_writer.begin_struct(name);
            }

            void System.IDisposable.Dispose() {
                m_writer.end_struct_or_array();
            }

            private ArchiveWriter m_writer;
        }

        public struct ArrayScope : System.IDisposable {
            internal ArrayScope(ArchiveWriter writer, string name = null) {
                m_writer = writer;
                m_writer.begin_array(name);
            }

            void System.IDisposable.Dispose() {
                m_writer.end_struct_or_array();
            }
            private ArchiveWriter m_writer;
        }

        public ArchiveWriter() {
            m_stack.Push(m_data);
        }

        public void begin_struct(string name = null) {
            var container = m_stack.Peek();
            var value = new Struct();
            if (name != null) {
                container.add(get_string_index(name), value);
            } else {
                container.add(value);
            }
            m_stack.Push(value);
        }



        public StructScope struct_scope(string name = null) {
            return new StructScope(this, name);
        }

        public void begin_array(string name = null) {
            var container = m_stack.Peek();
            var value = new Vector();
            if (name != null) {
                container.add(get_string_index(name), value);
            } else {
                container.add(value);
            }
            m_stack.Push(value);
        }

        public ArrayScope array_scope(string name = null) {
            return new ArrayScope(this, name);
        }

        public void end_struct_or_array() {
            m_stack.Pop();
        }

        internal void _write<T>(T value, string name = null) where T : struct, IArchive {
            var container = m_stack.Peek();
            if (name != null) {
                container.add(get_string_index(name), value);
            } else {
                container.add(value);
            }
        }

        internal void _write(INumber value, string name = null) {
            var container = m_stack.Peek();
            if (name != null) {
                container.add(get_string_index(name), value);
            } else {
                container.add(value);
            }
        }

        public void write(byte value, string name = null) {
            _write(new PositiveByte(value), name);
        }

        public void write(sbyte value, string name = null) {
            if (value >= 0) {
                _write(new PositiveByte((byte)value), name);
            } else {
                _write(new NegativeByte((byte)-value), name);
            }
        }

        public void write(ushort value, string name = null) {
            _write(Factory.create_number((ulong)value), name);
        }

        public void write(short value, string name = null) {
            _write(Factory.create_number(value), name);
        }

        public void write(uint value, string name = null) {
            _write(Factory.create_number((ulong)value), name);
        }

        public void write(int value, string name = null) {
            _write(Factory.create_number(value), name);
        }

        public void write(ulong value, string name = null) {
            _write(Factory.create_number(value), name);
        }

        public void write(long value, string name = null) {
            _write(Factory.create_number(value), name);
        }

        public void write(bool value, string name = null) {
            _write(new Bool { value = value }, name);
        }

        public void write(string value, string name = null) {
            _write(new StringIndex(get_string_index(value)), name);
        }

        public void write(float value, string name = null) {
            _write(new Float { value = value }, name);
        }

        public void write(double value, string name = null) {
            _write(new Double { value = value }, name);
        }

        public void write(UnityEngine.Vector2 value, string name = null) {
            _write(new Vec2 { value = value }, name);
        }

        public void write(UnityEngine.Vector2Int value, string name = null) {
            _write(new Vec2Int { value = value }, name);
        }

        public void write(System.Type value, string name = null) {
            if (value != null) {
                _write(new TypeIndex(get_type_index(value)), name);
            } else {
                _write(new Null(), name);
            }
        }

        public void write(byte[] value, string name = null) {
            if (value != null) {
                _write(new Bytes { bytes = value }, name);
            } else {
                _write(new Null(), name);
            }
        }

        public void write<T>(T value, string name = null) where T : class, IArchiver {
            if (value != null) {
                _write(new ObjectIndex(get_object_index(value)), name);
            } else {
                _write(new Null(), name);
            }
        }

        private uint get_string_index(string value) {
            if (value == null) {
                value = string.Empty;
            }
            if (!m_string_indices.TryGetValue(value, out var index)) {
                index = (uint)m_strings.Count;
                m_strings.Add(value);
                m_string_indices.Add(value, index);
            }
            return index;
        }

        private uint get_object_index(IArchiver obj) {
            if (!m_indices.TryGetValue(obj, out var index)) {
                index = (uint)m_objects.Count;
                m_objects.Add((obj, get_type_index(obj.GetType())));
                m_indices.Add(obj, index);
            }
            return index;
        }

        private uint get_type_index(System.Type type) {
            if (!m_object_type_indices.TryGetValue(type, out var index)) {
                index = (uint)m_object_types.Count;
                m_object_types.Add(type);
                m_object_type_indices.Add(type, index);
            }
            return index;
        }

        public void archive_objects() {
            for (int i = m_object_data.Count; i < m_objects.Count; ++i) {
                var obj = m_objects[i].Item1;
                m_stack.Clear();
                var data = new Struct();
                m_stack.Push(data);
                obj.archive(this);
                m_object_data.Add(data);
            }
            m_stack.Clear();
            m_stack.Push(m_data);
        }

        public void write_to(BinaryWriter writer) {
            writer.write_compressed_uint((ulong)m_object_types.Count);
            foreach (var type in m_object_types) {
                writer.write_string($"{type.FullName}, {type.Assembly.GetName().Name}");
            }
            writer.write_compressed_uint((ulong)m_strings.Count);
            foreach (var s in m_strings) {
                writer.write_string(s);
            }
            writer.write_compressed_uint((ulong)m_objects.Count);
            for (int i = 0; i < m_objects.Count; ++i) {
                writer.write_compressed_uint(m_objects[i].Item2);
                if (i < m_object_data.Count) {
                    m_object_data[i].save(writer);
                } else {
                    Struct.save_empty(writer);
                }
            }
            m_data.save(writer);
        }

        Dictionary<IArchiver, uint> m_indices = new Dictionary<IArchiver, uint>();
        System.Collections.Generic.List<(IArchiver, uint)> m_objects = new System.Collections.Generic.List<(IArchiver, uint)>();

        Dictionary<System.Type, uint> m_object_type_indices = new Dictionary<System.Type, uint>();
        System.Collections.Generic.List<System.Type> m_object_types = new System.Collections.Generic.List<System.Type>();

        Dictionary<string, uint> m_string_indices = new Dictionary<string, uint>();
        System.Collections.Generic.List<string> m_strings = new System.Collections.Generic.List<string>();

        Stack<IArchiveContainer> m_stack = new Stack<IArchiveContainer>();
        Struct m_data = new Struct();
        System.Collections.Generic.List<Struct> m_object_data = new System.Collections.Generic.List<Struct>();
    }

}
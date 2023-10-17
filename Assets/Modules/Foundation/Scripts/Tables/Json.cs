
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Foundation.Tables {

    public enum JsonValueType {
        Null,
        Boolean,
        Number,
        String,
        Array,
        Object,
    }

    public interface IJsonValue {
        JsonValueType type { get; }
        void load(BinaryReader reader, string[] strings);
        void skip_by_seek(BinaryReader reader);
        void skip_by_read(BinaryReader reader);

        bool try_get_bool(out bool val);
        bool try_get_int(out int val);
        bool try_get_float(out float val);
        bool try_get_string(out string val);
        bool try_get_array(out JsonArray val);
        bool try_get_object(out JsonObject val);
        bool try_convert_to(System.Type type, out object value);
    }

    public class JsonNull : IJsonValue {
        public JsonValueType type => JsonValueType.Null;
        public static readonly JsonNull instance = new JsonNull();

        void IJsonValue.load(BinaryReader reader, string[] strings) {}
        void IJsonValue.skip_by_seek(BinaryReader reader) {}
        void IJsonValue.skip_by_read(BinaryReader reader) {}
        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = 0; return false; }
        bool IJsonValue.try_get_float(out float val) { val = 0; return false; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            value = null;
            if (type.IsClass) {
                return true;
            }
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>)) {
                return true;
            }
            return false;
        }

        private JsonNull() { }
    }

    public class JsonBoolean : IJsonValue {
        public bool value;
        public JsonValueType type => JsonValueType.Boolean;

        void IJsonValue.load(BinaryReader reader, string[] strings) {}
        void IJsonValue.skip_by_seek(BinaryReader reader) {}
        void IJsonValue.skip_by_read(BinaryReader reader) {}
        bool IJsonValue.try_get_bool(out bool val) { val = value; return true; }
        bool IJsonValue.try_get_int(out int val) { val = 0; return false; }
        bool IJsonValue.try_get_float(out float val) { val = 0; return false; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            if (type.IsAssignableFrom(typeof(bool))) {
                value = this.value;
                return true;
            }
            var ctor = type.GetConstructor(new System.Type[] { typeof(bool) });
            if (ctor != null) {
                value = ctor.Invoke(new object[] { this.value });
                return true;
            }
            value = null;
            return false;
        }
    }

    public class JsonInteger : IJsonValue {
        public int value;
        public JsonValueType type => JsonValueType.Number;

        void IJsonValue.load(BinaryReader reader, string[] strings) {
            value = (int)CompressedInt.decompress_int(reader);
        }
        void IJsonValue.skip_by_seek(BinaryReader reader) { 
            CompressedInt.decompress_int(reader);
        }
        void IJsonValue.skip_by_read(BinaryReader reader) {
            CompressedInt.decompress_int(reader);
        }

        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = value; return true; }
        bool IJsonValue.try_get_float(out float val) { val = value; return true; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            if (type.IsAssignableFrom(typeof(int))) {
                value = this.value;
                return true;
            }
            if (type.IsAssignableFrom(typeof(float))) {
                value = (float)this.value;
                return true;
            }
            var ctor = type.GetConstructor(new System.Type[] { typeof(int) });
            if (ctor != null) {
                value = ctor.Invoke(new object[] { this.value });
                return true;
            }
            ctor = type.GetConstructor(new System.Type[] { typeof(float) });
            if (ctor != null) {
                value = ctor.Invoke(new object[] { (float)this.value });
                return true;
            }
            value = null;
            return false;
        }
    }

    public class JsonFloating : IJsonValue {
        public float value;
        public JsonValueType type => JsonValueType.Number;

        void IJsonValue.load(BinaryReader reader, string[] strings) {
            value = reader.ReadSingle();
        }

        void IJsonValue.skip_by_seek(BinaryReader reader) {
            reader.BaseStream.Seek(4, SeekOrigin.Current);
        }
        void IJsonValue.skip_by_read(BinaryReader reader) {
            reader.ReadUInt32();
        }

        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = (int)value; return true; }
        bool IJsonValue.try_get_float(out float val) { val = value; return true; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            if (type.IsAssignableFrom(typeof(float))) {
                value = this.value;
                return true;
            }
            var ctor = type.GetConstructor(new System.Type[] { typeof(float) });
            if (ctor != null) {
                value = ctor.Invoke(new object[] { this.value });
                return true;
            }
            value = null;
            return false;
        }
    }

    public class JsonString : IJsonValue {
        public string value;
        public JsonValueType type => JsonValueType.String;

        void IJsonValue.load(BinaryReader reader, string[] strings) {
            value = FieldReader.read_indexed_string(reader, strings);
        }

        void IJsonValue.skip_by_seek(BinaryReader reader) {
            CompressedInt.decompress_uint(reader);
        }
        void IJsonValue.skip_by_read(BinaryReader reader) {
            CompressedInt.decompress_uint(reader);
        }

        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = 0; return false; }
        bool IJsonValue.try_get_float(out float val) { val = 0; return false; }
        bool IJsonValue.try_get_string(out string val) { val = value; return true; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            if (type.IsAssignableFrom(typeof(string))) {
                value = this.value;
                return true;
            }
            var ctor = type.GetConstructor(new System.Type[] { typeof(string) });
            if (ctor != null) {
                value = ctor.Invoke(new object[] { this.value });
                return true;
            }
            value = null;
            return false;
        }
    }

    public class JsonArray : IJsonValue, IReadOnlyList<IJsonValue> {
        public JsonValueType type => JsonValueType.Array;

        public List<IJsonValue>.Enumerator GetEnumerator() { return m_items.GetEnumerator(); }
        
        IEnumerator<IJsonValue> IEnumerable<IJsonValue>.GetEnumerator() { return GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count => m_items.Count;
        public IJsonValue this[int index] => m_items[index];

        void IJsonValue.load(BinaryReader reader, string[] strings) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_items = new List<IJsonValue>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            if (m_items.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            foreach (var val in m_items) {
                val.load(reader, strings);
            }
        }

        void IJsonValue.skip_by_seek(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_items = new List<IJsonValue>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            if (m_items.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            foreach (var val in m_items) {
                val.skip_by_seek(reader);
            }
        }

        void IJsonValue.skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_items = new List<IJsonValue>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            if (m_items.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_items.Add(val);
            }
            foreach (var val in m_items) {
                val.skip_by_read(reader);
            }
        }

        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = 0; return false; }
        bool IJsonValue.try_get_float(out float val) { val = 0; return false; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = this; return true; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = null; return false; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            if (type.IsArray && type.GetArrayRank() == 1) {
                var et = type.GetElementType();
                var arr = System.Activator.CreateInstance(type, new object[] { m_items.Count }) as System.Array;
                int index = 0;
                foreach (var item in m_items) {
                    if (item.try_convert_to(et, out object val)) {
                        arr.SetValue(val, index);
                    }
                    ++index;
                }
                value = arr;
                return true;
            }
            value = null;
            return false;
        }
        private List<IJsonValue> m_items;
    }

    public class JsonObject : IJsonValue, IReadOnlyList<(string, IJsonValue)> {
        
        public void overwrite(object obj) {
            var ty = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var (name, val) in m_members) {
                var field = ty.GetField(name, flags);
                if (field != null) {
                    if (val.try_convert_to(field.FieldType, out object value)) {
                        field.SetValue(obj, value);
                    }
                }
            }
        }

        public T create_object<T>() {
            return (T)create_object(typeof(T));
        }

        public object create_object(System.Type type) {
            ((IJsonValue)this).try_convert_to(type, out object value);
            return value;
        }

        public IJsonValue this[string name] {
            get {
                foreach (var (n, v) in m_members) {
                    if (n == name) {
                        return v;
                    }
                }
                return null;
            }
        }

        JsonValueType IJsonValue.type => JsonValueType.Object;
        void IJsonValue.load(BinaryReader reader, string[] strings) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_members = new List<(string, IJsonValue)>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            if (m_members.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            for (int i = 0; i < len; ++i) {
                var (_, val) = m_members[i];
                var name = FieldReader.read_indexed_string(reader, strings);
                val.load(reader, strings);
                m_members[i] = (name, val);
            }
        }

        void IJsonValue.skip_by_seek(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_members = new List<(string, IJsonValue)>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            if (m_members.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            for (int i = 0; i < len; ++i) {
                CompressedInt.decompress_uint(reader);
                m_members[i].Item2.skip_by_seek(reader);
            }
        }

        void IJsonValue.skip_by_read(BinaryReader reader) {
            var len = (int)CompressedInt.decompress_uint(reader);
            m_members = new List<(string, IJsonValue)>(len);
            int c = len / 2;
            var types = reader.ReadBytes(len % 2 != 0 ? c + 1 : c);
            for (int i = 0; i < c; ++i) {
                var t = types[i];
                var val = JsonUtility.create_value(t & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
                val = JsonUtility.create_value(t >> 4);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            if (m_members.Count < len) {
                var val = JsonUtility.create_value(types[c] & 0xF);
                if (val == null) { throw new IOException(); }
                m_members.Add((null, val));
            }
            for (int i = 0; i < len; ++i) {
                CompressedInt.decompress_uint(reader);
                m_members[i].Item2.skip_by_read(reader);
            }
        }

        bool IJsonValue.try_get_bool(out bool val) { val = false; return false; }
        bool IJsonValue.try_get_int(out int val) { val = 0; return false; }
        bool IJsonValue.try_get_float(out float val) { val = 0; return false; }
        bool IJsonValue.try_get_string(out string val) { val = string.Empty; return false; }
        bool IJsonValue.try_get_array(out JsonArray val) { val = null; return false; }
        bool IJsonValue.try_get_object(out JsonObject val) { val = this; return true; }
        bool IJsonValue.try_convert_to(System.Type type, out object value) {
            try {
                value = System.Activator.CreateInstance(type);
            } catch (System.Exception) {
                value = null;
                return false;
            }
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var (name, val) in m_members) {
                var field = type.GetField(name, flags);
                if (field != null) {
                    if (val.try_convert_to(field.FieldType, out object obj)) {
                        field.SetValue(value, obj);
                    }
                }
            }
            return true;
        }

        public int Count => m_members.Count;
        public (string, IJsonValue) this[int index] => m_members[index];
        public List<(string, IJsonValue)>.Enumerator GetEnumerator() { return m_members.GetEnumerator(); }
        IEnumerator<(string, IJsonValue)> IEnumerable<(string, IJsonValue)>.GetEnumerator() { return GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private List<(string, IJsonValue)> m_members;
    }

    public static class JsonUtility {
        public static IJsonValue create_value(int type) {
            switch (type) {
                case 0: return JsonNull.instance;
                case 1: return new JsonBoolean { value = true };
                case 2: return new JsonBoolean { value = false };
                case 3: return new JsonInteger();
                case 4: return new JsonFloating();
                case 5: return new JsonString();
                case 6: return new JsonArray();
                case 7: return new JsonObject();
                default: return null;
            }
        }
    }

}
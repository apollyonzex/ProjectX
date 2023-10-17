using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Common_Formal
{
    public class StringObjectMap : MonoBehaviour
    {

        [System.Serializable]
        public struct Item
        {
            public string key;
            public Object value;
        }

        [SerializeField]
        private Item[] m_items;

        private Dictionary<string, List<(Object obj, int index)>> m_string_objects;

        public int count => m_items.Length;
        public Item get_item(int index) => m_items[index];

        public class Reference<T> where T : class
        {
            public readonly StringObjectMap prototype;
            private List<(FieldInfo fi, int idx)> m_fields = new List<(FieldInfo fi, int idx)>();

            public Reference(StringObjectMap prototype)
            {
                this.prototype = prototype;
                prototype.build_string_objects();
                foreach (var fi in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                {
                    if (prototype.m_string_objects.TryGetValue(fi.Name, out var list))
                    {
                        foreach (var e in list)
                        {
                            if (fi.FieldType.IsAssignableFrom(e.obj.GetType()))
                            {
                                m_fields.Add((fi, e.index));
                                break;
                            }
                        }
                    }
                }
            }

            public StringObjectMap instantiate(T target)
            {
                var ret = Instantiate(prototype);
                if (target != null)
                {
                    foreach (var e in m_fields)
                    {
                        e.fi.SetValue(target, ret.m_items[e.idx].value);
                    }
                }
                return ret;
            }

            public StringObjectMap instantiate(T target, Transform parent, bool in_world_space)
            {
                var ret = Instantiate(prototype, parent, in_world_space);
                if (target != null)
                {
                    foreach (var e in m_fields)
                    {
                        e.fi.SetValue(target, ret.m_items[e.idx].value);
                    }
                }
                return ret;
            }
        }

        public bool try_get_object(string name, out Object obj)
        {
            build_string_objects();
            if (m_string_objects.TryGetValue(name ?? string.Empty, out var objs))
            {
                obj = objs[0].obj;
                return true;
            }
            obj = null;
            return false;
        }

        public bool try_get_objects(string name, out List<(Object obj, int inex)> objects)
        {
            build_string_objects();
            return m_string_objects.TryGetValue(name ?? string.Empty, out objects);
        }

        public bool try_get_object<T>(string name, out T obj) where T : class
        {
            if (try_get_objects(name, out var list))
            {
                foreach (var e in list)
                {
                    if (e is T t)
                    {
                        obj = t;
                        return true;
                    }
                }
            }
            obj = null;
            return false;
        }

        public bool try_get_object(string name, System.Type type, out Object obj)
        {
            if (try_get_objects(name, out var list))
            {
                foreach (var e in list)
                {
                    if (type.IsAssignableFrom(e.obj.GetType()))
                    {
                        obj = e.obj;
                        return true;
                    }
                }
            }
            obj = null;
            return false;
        }

        public Reference<T> create_reference<T>() where T : class
        {
            return new Reference<T>(this);
        }

        private void build_string_objects()
        {
            if (m_string_objects == null)
            {
                m_string_objects = new Dictionary<string, List<(Object obj, int index)>>();
                if (m_items != null)
                {
                    var index = 0;
                    foreach (var item in m_items)
                    {
                        if (item.value != null)
                        {
                            var key = item.key ?? string.Empty;
                            if (!m_string_objects.TryGetValue(key, out var list))
                            {
                                list = new List<(Object obj, int index)>();
                                m_string_objects.Add(key, list);
                            }
                            list.Add((item.value, index));
                        }
                        ++index;
                    }
                }
            }
        }
    }

}
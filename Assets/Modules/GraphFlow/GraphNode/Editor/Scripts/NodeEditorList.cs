
using System.Collections.Generic;

namespace GraphNode.Editor {

    public interface INamedNodeEditor<T> where T : class, INamedNodeEditor<T> {
        string name { get; }
        int name_index { get; set; }
        event System.Action<T> on_removed;

        NodeView view { get; }
        Node node { get; }
    }

    public class NodeEditorList<T> where T : class, INamedNodeEditor<T> {

        public void add(T item, bool rise_event) {
            m_items.Add(item);
            if (rise_event) {
                var undo = item.view.graph.undo;
                if (undo.operating) {
                    if (!m_names_dirty) {
                        m_names_dirty = true;
                        undo.operating_delay_call += build_names;
                    }
                } else {
                    build_names();
                }
                added?.Invoke(item);
            }
        }

        public bool remove(T item) {
            if (m_items.Remove(item)) {
                item.name_index = 0;
                var undo = item.view.graph.undo;
                if (undo.operating) {
                    if (!m_names_dirty) {
                        m_names_dirty = true;
                        undo.operating_delay_call += build_names;
                    }
                } else {
                    build_names();
                }
                removed?.Invoke(item);
                return true;
            }
            return false;
        }

        public void notify_name_changed(T item) {
            if (!m_names_dirty) {
                m_names[item.name_index] = item.name;
            }
        }


        public event System.Action names_built;
        public event System.Action names_built_call;
        public event System.Action<T> added;
        public event System.Action<T> removed;

        public bool names_dirty => m_names_dirty;
        public string[] names => m_names;
        public Foundation.ReadOnlyList<T> items => m_items;

        public bool try_get_by_node(Node node, out T editor) {
            foreach (var item in m_items) {
                if (item.node == node) {
                    editor = item;
                    return true;
                }
            }
            editor = null;
            return false;
        }

        public T get_by_name_index(int name_index) {
            return name_index > 0 ? m_items[name_index - 1] : null;
        }

        public void build_names() {
            m_names_dirty = false;
            if (m_items.Count != 0) {
                m_names = new string[m_items.Count + 1];
                m_names[0] = s_inital[0];
                var index = 1;
                foreach (var item in m_items) {
                    m_names[index] = item.name;
                    item.name_index = index;
                    ++index;
                }
            } else {
                m_names = s_inital;
            }
            if (names_built_call != null) {
                names_built_call.Invoke();
                names_built_call = null;
            }
            names_built?.Invoke();
        }

        List<T> m_items = new List<T>();
        string[] m_names = s_inital;
        bool m_names_dirty = false;

        static readonly string[] s_inital = new string[] { "<None>" };
    }
}
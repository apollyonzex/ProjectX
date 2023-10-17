
using UnityEditor;
using UnityEngine;
using GraphNode.Editor;

namespace DialogFlow.Editor {

    [PropertyEditor(typeof(PageNodeBase))]
    public class PropertyPageNodeBaseEditor : GenericPropertyEditor {

        public override void on_inspector_disable() {
            
        }

        public override void on_inspector_enable() {
            
        }

        public override void on_graph_open() {
            if (m_graph is DialogGraphEditor ge) {
                ge.on_page_removed += on_page_removed;
            }
        }

        public override void on_node_add_to_graph() {
            if (m_graph is DialogGraphEditor ge) {
                ge.on_page_removed += on_page_removed;
            }
        }

        public override void on_node_remove_from_graph() {
            if (m_graph is DialogGraphEditor ge) {
                ge.on_page_removed -= on_page_removed;
            }
        }

        private void on_page_removed(PageNodeBaseEditor page) {
            if (m_graph is DialogGraphEditor ge) {
                validate(ge);
            }
            if (m_page == page) {
                if (!m_graph.view.undo.operating) {
                    m_graph.view.undo.record(new ChangeTarget { editor = this, new_value = null, old_value = m_page });
                }
                set_value(null);
                m_page = null;
                notify_changed(false);
            }
        }

        public override void on_inspector_gui() {
            if (m_graph is DialogGraphEditor ge) {
                validate(ge);
                EditorGUI.BeginChangeCheck();
                var index = m_page != null ? m_page.page_index : 0;
                index = EditorGUILayout.Popup(name, index, ge.page_names);
                if (EditorGUI.EndChangeCheck()) {
                    var new_value = ge.get_page(index);
                    ge.view.undo.record(new ChangeTarget { editor = this, old_value = m_page, new_value = new_value });
                    m_page = new_value;
                    set_value(new_value?.node);
                    notify_changed(true);
                }
            }
        }

        public override void on_body_gui() {
            if (m_graph is DialogGraphEditor ge) {
                if (m_show_in_body != null) {
                    validate(ge);
                    var index = m_page != null ? m_page.page_index : 0;
                    if (m_show_in_body.format == null) {
                        GUILayout.Label(ge.page_names[index]);
                    } else {
                        GUILayout.Label(string.Format(m_show_in_body.format, ge.page_names[index]));
                    }
                }
            }
        }

        private void validate(DialogGraphEditor graph) {
            var value = get_value() as PageNodeBase;
            if (value != null && m_page == null) {
                m_page = graph.get_page(value);
            }
        }

        private PageNodeBaseEditor m_page;


        private class ChangeTarget : GraphUndo.ChangeValue<PageNodeBaseEditor> {
            public PropertyPageNodeBaseEditor editor;

            protected override void set_value(ref PageNodeBaseEditor old_value, ref PageNodeBaseEditor value) {
                editor.set_value(value?.node);
                editor.m_page = value;
                editor.notify_changed(false);
            }
        }
    }
}
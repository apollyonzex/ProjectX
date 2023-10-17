
using GraphNode.Editor;
using UnityEditor;
using UnityEngine;

namespace StateFlow.Editor {

    [PropertyEditor(typeof(StateNodeBase))]
    public class StateNodeBasePropertyEditor : GenericPropertyEditor {

        public StateNodeBaseEditor value_editor => m_value_editor;

        public override void on_graph_open() {
            if (m_graph is StateGraphEditor ge) {
                ge.state_names_done += delay_init_value_editor;
            }
        }

        public override void on_node_add_to_graph() {
            if (m_graph.view.undo.operating) {
                m_graph.view.undo.operating_delay_call += () => {
                    if (m_graph is StateGraphEditor ge) {
                        if (ge.state_names_dirty) {
                            ge.state_names_done += delay_init_value_editor;
                        } else {
                            delay_init_value_editor();
                        }
                    }
                };
            } else {
                init_value_editor();
            }
        }

        public override void on_node_remove_from_graph() {
            if (m_value_editor != null) {
                m_value_editor.on_removed -= on_value_removed;
                m_value_editor = null;
            }
        }

        public override void on_inspector_enable() {

        }

        public override void on_inspector_disable() {

        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_graph is StateGraphEditor ge) {
                    var index = 0;
                    if (m_value_editor != null) {
                        index = m_value_editor.name_index;
                    }
                    var s = ge.state_names[index];
                    if (m_show_in_body.format == null) {
                        GUILayout.Label(s);
                    } else {
                        GUILayout.Label(string.Format(m_show_in_body.format, s));
                    }
                }
            }
        }

        public override void on_inspector_gui() {
            if (m_graph is StateGraphEditor ge) {
                var index = 0;
                if (m_value_editor != null) {
                    index = m_value_editor.name_index;
                }

                var new_index = EditorGUILayout.Popup(name, index, ge.state_names);
                if (new_index != index) {
                    StateNodeBaseEditor new_value = null;
                    if (new_index != 0) {
                        ge.state_dict.TryGetValue(ge.state_names[new_index], out new_value);
                    }
                    var cmd = new ChangeValue { editor = this, old_value = m_value_editor, new_value = new_value };
                    m_graph.view.undo.record(cmd);
                    if (m_value_editor != null) {
                        m_value_editor.on_removed -= on_value_removed;
                    }
                    if (new_value != null) {
                        new_value.on_removed += on_value_removed;
                        set_value(new_value.node);
                    } else {
                        set_value(null);
                    }
                    m_value_editor = new_value;
                    notify_changed(true);
                }
            }
        }

        void delay_init_value_editor() {
            init_value_editor();
        }

        void init_value_editor() {
            if (m_graph is StateGraphEditor ge) {
                var value = (StateNodeBase)get_value();
                if (value != null) {
                    if (ge.state_dict.TryGetValue(value.name, out m_value_editor)) {
                        m_value_editor.on_removed += on_value_removed;
                    } else {
                        set_value(null);
                    }
                }
            }
        }

        void on_value_removed() {
            var by_user = !m_graph.view.undo.operating;
            if (by_user) {
                m_graph.view.undo.record(new ChangeValue { editor = this, old_value = m_value_editor, new_value = null });
            }
            m_value_editor.on_removed -= on_value_removed;
            m_value_editor = null;
            set_value(null);
            notify_changed(by_user);
        }

        StateNodeBaseEditor m_value_editor;

        class ChangeValue : GraphUndo.ChangeValue<StateNodeBaseEditor> {
            public StateNodeBasePropertyEditor editor;

            protected override void set_value(ref StateNodeBaseEditor old_value, ref StateNodeBaseEditor new_value) {
                if (old_value != null) {
                    old_value.on_removed -= editor.on_value_removed;
                }
                if (new_value != null) {
                    new_value.on_removed += editor.on_value_removed;
                    editor.set_value(new_value.node);
                } else {
                    editor.set_value(null);
                }
                editor.m_value_editor = new_value;
                editor.notify_changed(false);
            }
        }
    }
}
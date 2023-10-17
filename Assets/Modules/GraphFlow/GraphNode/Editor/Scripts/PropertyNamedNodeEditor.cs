

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GraphNode.Editor {
    public abstract class PropertyNamedNodeEditor<T> : GenericPropertyEditor where T : class, INamedNodeEditor<T> {

        public abstract NodeEditorList<T> node_editor_list { get; }

        public T value_editor => m_value_editor;

        public override void on_graph_open() {
            var list = node_editor_list;
            list.names_built_call += delay_init_value_editor;
        }

        public override void on_node_add_to_graph() {
            if (m_graph.view.undo.operating) {
                m_graph.view.undo.operating_delay_call += () => {
                    var list = node_editor_list;
                    if (list.names_dirty) {
                        list.names_built_call += delay_init_value_editor;
                    } else {
                        delay_init_value_editor();
                    }
                };
            } else {
                init_value_editor();
            }
        }

        public override void on_node_remove_from_graph() {
            if (m_value_editor != null) {
                on_lose_value_editor();
                m_value_editor = null;
            }
        }

        public override void on_node_duplicated_done(List<NodeView> source_nodes, List<NodeView> duplicated_nodes) {
            var node_index = duplicated_nodes.IndexOf(m_node.view);
            var source_node_editor = source_nodes[node_index].editor;
            if (source_node_editor is GenericNodeEditor ne && ne.try_get_property(raw_name, out var pe) && pe is PropertyNamedNodeEditor<T> pne) {
                if (pne.value_editor != null) {
                    node_index = source_nodes.IndexOf(pne.value_editor.view);
                    if (node_index >= 0) {
                        m_value_editor = duplicated_nodes[node_index].editor as T;
                        if (m_value_editor != null) {
                            set_value(m_value_editor.node);
                            notify_changed(false);
                        }
                    }
                }
            }
        }

        public override void on_inspector_enable() {

        }

        public override void on_inspector_disable() {

        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                string s = get_display_in_body();
                if (s != null) {
                    if (m_show_in_body.format == null) {
                        GUILayout.Label(s);
                    } else {
                        GUILayout.Label(string.Format(m_show_in_body.format, s));
                    }
                }
            }
        }

        public override void on_inspector_gui() {
            var index = 0;
            if (m_value_editor != null) {
                index = m_value_editor.name_index;
            }
            var list = node_editor_list;
            var new_index = EditorGUILayout.Popup(name, index, list.names);
            if (new_index != index) {
                T new_value = list.get_by_name_index(new_index);
                var old_value = m_value_editor;
                if (m_value_editor != null) {
                    on_lose_value_editor();
                }
                m_value_editor = new_value;
                if (m_value_editor != null) {
                    on_got_value_editor();
                    set_value(new_value.node);
                } else {
                    set_value(null);
                }

                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                var cmd = new ChangeValue { editor = this, associated = undo.cancel_group(), old_value = old_value, new_value = new_value };
                m_graph.view.undo.record(cmd);
            }
        }

        public void on_list_item_gui(Rect rect) {
            var index = 0;
            if (m_value_editor != null) {
                index = m_value_editor.name_index;
            }
            var list = node_editor_list;
            var new_index = EditorGUI.Popup(rect, name, index, list.names);
            if (new_index != index) {
                T new_value = list.get_by_name_index(new_index);
                var old_value = m_value_editor;
                if (m_value_editor != null) {
                    on_lose_value_editor();
                }
                m_value_editor = new_value;
                if (m_value_editor != null) {
                    on_got_value_editor();
                    set_value(new_value.node);
                } else {
                    set_value(null);
                }

                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                var cmd = new ChangeValue { editor = this, associated = undo.cancel_group(), old_value = old_value, new_value = new_value };
                m_graph.view.undo.record(cmd);
            }
        }

        protected virtual void delay_init_value_editor() {
            init_value_editor();
        }

        protected void init_value_editor() {
            var value = get_value() as Node;
            if (value != null) {
                if (node_editor_list.try_get_by_node(value, out m_value_editor)) {
                    on_got_value_editor();
                } else {
                    set_value(null);
                }
            }
        }

        protected virtual void on_got_value_editor() {
            m_value_editor.on_removed += on_value_removed;
        }

        protected virtual void on_lose_value_editor() {
            m_value_editor.on_removed -= on_value_removed;
        }

        protected virtual string get_display_in_body() {
            if (m_value_editor != null) {
                return m_value_editor.name;
            }
            return node_editor_list.names[0];
        }

        protected T m_value_editor;

        protected virtual void on_value_removed(T ne) {
            if (m_value_editor == null) {
                return;
            }
            var old_value = m_value_editor;
            on_lose_value_editor();
            m_value_editor = null;
            set_value(null);
            if (m_graph.view.undo.operating) {
                notify_changed(false);
            } else {
                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                var cmd = new ChangeValue { editor = this, associated = undo.cancel_group(), old_value = old_value };
                m_graph.view.undo.record(cmd);
            }
        }

        class ChangeValue : GraphUndo.ICommand {
            public PropertyNamedNodeEditor<T> editor;
            public GraphUndo.CommandGroup associated;
            public T old_value;
            public T new_value;

            public int dirty_count => 1;

            public void undo() {
                if (new_value != null) {
                    editor.on_lose_value_editor();
                }
                editor.m_value_editor = old_value;
                if (old_value != null) {
                    editor.on_got_value_editor();
                    editor.set_value(old_value.node);
                } else {
                    editor.set_value(null);
                }
                associated?.undo();
                editor.notify_changed(false);
            }

            public void redo() {
                if (old_value != null) {
                    editor.on_lose_value_editor();
                }
                editor.m_value_editor = new_value;
                if (new_value != null) {
                    editor.on_got_value_editor();
                    editor.set_value(new_value.node);
                } else {
                    editor.set_value(null);
                }
                associated?.redo();
                editor.notify_changed(false);
            }
        }
    }
}
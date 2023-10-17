
using GraphNode;
using GraphNode.Editor;
using System.Reflection;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using System;

namespace BehaviourFlow.Editor {

    [PropertyEditor(typeof(List<BTChildNodePort>))]
    public class BTChildrenEditor : GenericPropertyEditor {

        public List<BTChildNodePort> target { get; private set; }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init();
        }

        public override void on_inspector_disable() {
            
        }

        public override void on_inspector_enable() {
            
        }

        public override void on_inspector_gui() {
            m_list.DoLayoutList();
            if (GUILayout.Button("Reorder in Vertical")) {
                reorder_in_vertical();
            }
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                var ports = new List<BTChildNodePort>(this.target.Count);
                /*
                for (int i = 0; i < this.target.Count; ++i) {
                    ports.Add(new BTChildNodePort());
                }
                */
                m_fi.SetValue(target, ports);
            }
        }

        private ReorderableList m_list;

        private void init() {
            target = get_value() as List<BTChildNodePort>;
            if (target == null) {
                target = new List<BTChildNodePort>();
                set_value(target);
            }
            m_list = new ReorderableList(target, typeof(BTChildNodePort), true, true, false, false);
            m_list.footerHeight = 0;
            m_list.drawHeaderCallback = (rect) => GUI.Label(rect, name);
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_item;
        }

        private void on_reorder_item(ReorderableList list, int old_index, int new_index) {
            if (m_node is BTChildNodeWithChildrenEditor node) {
                var dp = target[old_index];
                node.get_dynamic_port_id(dp, out var old_view_index);
                var off = old_view_index - old_index;
                var view = node.view.dynamic_output_ports[old_view_index];
                var new_view_index = new_index + off;
                node.view.dynamic_output_ports.RemoveAt(old_view_index);
                node.view.dynamic_output_ports.Insert(new_view_index, view);
                node.view.size_changed = true;

                var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
                for (int i = min; i <= max; ++i) {
                    target[i].index = i + off;
                }
            }
            m_graph.view.undo.record(new ChangeIndex { editor = this, old_value = old_index, new_value = new_index });
            notify_changed(true);
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            GUI.Label(rect, target[index].index.ToString());
        }

        private void change_index(int old_index, int new_index) {
            if (m_node is BTChildNodeWithChildrenEditor node) {
                var dp = target[old_index];
                node.get_dynamic_port_id(dp, out var old_view_index);
                var view = node.view.dynamic_output_ports[old_view_index];
                var off = old_view_index - old_index;
                var new_view_index = new_index + off;
                target.RemoveAt(old_index);
                node.view.dynamic_output_ports.RemoveAt(old_view_index);
                target.Insert(new_index, dp);
                node.view.dynamic_output_ports.Insert(new_view_index, view);
                node.view.size_changed = true;

                var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
                for (int i = min; i <= max; ++i) {
                    target[i].index = i + off;
                }
            } else {
                var dp = target[old_index];
                target.RemoveAt(old_index);
                target.Insert(new_index, dp);
            }
        }

        public void reorder_in_vertical() {
            if (target.Count == 0) {
                return;
            }
            if (m_node is BTChildNodeWithChildrenEditor node) {
                var off = target[0].index;
                var dynamic_output_ports = node.view.dynamic_output_ports;
                target.Sort((a, b) => dynamic_output_ports[a.index].get_first_connection().input_position.y.CompareTo(dynamic_output_ports[b.index].get_first_connection().input_position.y));
                var old_indices = new int[target.Count];
                var reordered = false;
                for (int i = 0; i < target.Count; ++i) {
                    var e = target[i];
                    old_indices[i] = e.index;
                    var new_index = i + off;
                    if (e.index != new_index) {
                        e.index = new_index;
                        reordered = true;
                    }
                }
                if (reordered) {
                    var new_views = new OutputPortView[target.Count];
                    for (int i = 0; i < target.Count; ++i) {
                        new_views[i] = dynamic_output_ports[old_indices[i]];
                    }
                    for (int i = 0; i < target.Count; ++i) {
                        dynamic_output_ports[i + off] = new_views[i];
                    }
                    m_graph.view.undo.record(new ReorderInVertical { editor = this, offset = off, old_indices = old_indices });
                    node.view.size_changed = true;
                    notify_changed(true);
                }
            }
        }

        private class ChangeIndex : GraphUndo.ChangeValue<int> {
            public BTChildrenEditor editor;

            protected override void set_value(ref int old_value, ref int new_value) {
                editor.change_index(old_value, new_value);
                editor.notify_changed(false);
            }
        }

        private class ReorderInVertical : GraphUndo.ICommand {
            public BTChildrenEditor editor;
            public int offset;
            public int[] old_indices;

            int GraphUndo.ICommand.dirty_count => 1;

            void GraphUndo.ICommand.redo() {
                var target = editor.target.ToArray();
                for (int i = 0; i < target.Length; ++i) {
                    var e = target[old_indices[i] - offset];
                    e.index = offset + i;
                    editor.target[i] = e;
                }
                if (editor.m_node is BTChildNodeWithChildrenEditor node) {
                    var dynamic_output_ports = node.view.dynamic_output_ports;
                    var new_views = new OutputPortView[target.Length];
                    for (int i = 0; i < target.Length; ++i) {
                        new_views[i] = dynamic_output_ports[old_indices[i]];
                    }
                    for (int i = 0; i < target.Length; ++i) {
                        dynamic_output_ports[i + offset] = new_views[i];
                    }
                    editor.m_node.view.size_changed = true;
                }
                editor.notify_changed(false);
            }

            void GraphUndo.ICommand.undo() {
                var target = editor.target.ToArray();
                for (int i = 0; i < target.Length; ++i) {
                    target[i].index = old_indices[i];
                }
                for (int i = 0; i < target.Length; ++i) {
                    editor.target[old_indices[i] - offset] = target[i];
                }
                if (editor.m_node is BTChildNodeWithChildrenEditor node) {
                    var dynamic_output_ports = node.view.dynamic_output_ports;
                    var new_views = new OutputPortView[target.Length];
                    for (int i = 0; i < target.Length; ++i) {
                        new_views[i] = dynamic_output_ports[i + offset];
                    }
                    for (int i = 0; i < target.Length; ++i) {
                        dynamic_output_ports[old_indices[i]] = new_views[i];
                    }
                    editor.m_node.view.size_changed = true;
                }
                editor.notify_changed(false);
            }
        }
    }
}
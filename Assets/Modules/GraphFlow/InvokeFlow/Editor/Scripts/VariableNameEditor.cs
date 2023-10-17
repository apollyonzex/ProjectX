

using GraphNode.Editor;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GraphNode;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(VariableName))]
    public class VariableNameEditor : GenericPropertyEditor, ExpressionContextNodeEditor.IStackChange {

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            m_node = node as ExpressionContextNodeEditor;
            m_val = fi.GetValue(obj) as VariableName;
            if (m_val == null) {
                m_val = new VariableName();
                fi.SetValue(obj, m_val);
            }
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            m_node = node as ExpressionContextNodeEditor;
            m_val = (VariableName)obj ?? new VariableName();
        }

        public override object get_value() {
            return m_val.name;
        }

        public override void set_value(object value) {
            change_name((string)value);
        }

        private VariableName m_val;

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, new VariableName { name = m_val.name });
            }
        }

        public Variable target {
            get => m_target;
            set {
                if (m_target != value) {
                    m_target = value;
                    if (m_node is ExpressionContextNodeEditor ec) {
                        ec.notify_variable_name_changed(this);
                    }
                }
            }
        }

        public VariableName val => m_val;

        private Variable m_target = null;

        public override void on_body_gui() {
            
        }

        public override void on_inspector_enable() {
            
        }

        public override void on_inspector_disable() {
            
        }

        public override void on_inspector_gui() {
            var index = EditorGUILayout.Popup(name, m_index, m_names);
            check_change(index);
            if (m_target == null && m_val.name != string.Empty) {
                EditorGUILayout.HelpBox("Invalid target, please select new one, or repair connection", MessageType.Error);
            }
        }

        private void check_change(int index) {
            if (index != m_index) {
                if (index == 0) {
                    m_index = 0;
                    m_val.name = string.Empty;
                    m_val.stack_pos = -1;
                    target = null;
                } else {
                    var new_name = m_names[index];
                    if (m_node is ExpressionContextNodeEditor ec && ec.variable_dict.TryGetValue(new_name, out var info)) {
                        if (new_name != m_val.name) {
                            var old_name = m_val.name;
                            m_val.name = new_name;
                            m_graph.view.undo.begin_group();
                            notify_changed(true);
                            m_graph.view.undo.record(create_undo_command(old_name, new_name, m_graph.view.undo.cancel_group()));
                        }
                        m_index = index;
                        m_val.stack_pos = info.Item2;
                        target = info.Item1;
                    } else {
                        m_val.stack_pos = -1;
                        m_index = -1;
                        target = null;
                    }
                }
            }
        }

        public void on_list_element(Rect rect) {
            var index = EditorGUI.Popup(rect, name, m_index, m_names);
            check_change(index);
        }

        void ExpressionContextNodeEditor.IStackChange.on_stack_change(ExpressionContextNodeEditor node_editor) {
            m_names = new string[node_editor.variable_dict.Count + 1];
            m_names[0] =  "<none>";
            int i = 0;
            foreach (var kvp in node_editor.variable_dict) {
                m_names[++i] = kvp.Key;
            }
            if (m_val.name != string.Empty) {
                if (node_editor.variable_dict.TryGetValue(m_val.name, out var info)) {
                    m_val.stack_pos = info.Item2;
                    for (i = 0; i < m_names.Length; ++i) {
                        if (m_names[i] == m_val.name) {
                            m_index = i;
                            break;
                        }
                    }
                    target = info.Item1;
                } else {
                    m_val.stack_pos = -1;
                    m_index = -1;
                    target = null;
                }
            }
        }

        void change_name(string value) {
            m_val.name = value;
            if (value == string.Empty) {
                m_index = 0;
                m_val.stack_pos = -1;
                target = null;
            } else if (m_node is ExpressionContextNodeEditor ec && ec.variable_dict.TryGetValue(value, out var info)) {
                m_val.stack_pos = info.Item2;
                m_index = -1;
                for (int i = 0; i < m_names.Length; ++i) {
                    if (m_names[i] == m_val.name) {
                        m_index = i;
                        break;
                    }
                }
                target = info.Item1;
            } else {
                m_index = -1;
                m_val.stack_pos = -1;
                target = null;
            }
        }

        string[] m_names = new string[] { "<none>" };
        int m_index = 0;
    }

}
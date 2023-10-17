using GraphNode.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InvokeFlow.Editor {



    [NodeEditor(typeof(ExpressionContextNode))]
    public class ExpressionContextNodeEditor : InvokeNodeEditor {

        public interface IStackChange {
            void on_stack_change(ExpressionContextNodeEditor node_editor);
        }

        public override void attach(GraphEditor graph, GraphNode.Node node) {
            base.attach(graph, node);
        }

        protected override void on_stack_changed() {
            clear_stack();
            push_stack_frame(get_stack_frame());
            InvokeNodeEditor node = this;
            while (node.get_parent(out var parent, out var port)) {
                node = parent;
                push_stack_frame(node.get_port_stack_frame(port));
                push_stack_frame(node.get_stack_frame());
            }

            if (node.can_access_graph_variables) {
                if (view.graph.editor is InvokeGraphEditor ge) {
                    push_stack_frame(ge.stack_frame);
                }
            }

            foreach (var pe in m_properties) {
                if (pe is IStackChange sc) {
                    sc.on_stack_change(this);
                }
            }
        }

        public virtual void notify_variable_name_changed(VariableNameEditor vne) {

        }

        public virtual void notify_expression_changed(ExpressionEditor ee) {
            ee.build();
        }

        public virtual void notify_referenced_elements_changed() {
            foreach (var pe in m_properties) {
                if (pe is IStackChange sc) {
                    sc.on_stack_change(this);
                }
            }
        }

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            on_inspector_gui_inner();
            GUILayout.FlexibleSpace();
            s_variable_dict_visible = EditorGUILayout.BeginFoldoutHeaderGroup(s_variable_dict_visible, "Available Variables");
            if (s_variable_dict_visible) {
                GUILayout.BeginVertical(GUI.skin.box);
                if (m_runtime_data == null) {
                    foreach (var kvp in m_variable_dict) {
                        string ty = "";
                        switch (kvp.Value.Item1.type) {
                            case VariableType.Integer:
                                ty = "int";
                                break;
                            case VariableType.Floating:
                                ty = "float";
                                break;
                            case VariableType.Boolean:
                                ty = "bool";
                                break;
                        }
                        GUILayout.Label($"{kvp.Key}: {ty}");
                    }
                } else {
                    foreach (var kvp in m_variable_dict) {
                        string ty = "";
                        string val = "";
                        m_runtime_data.TryGetValue(kvp.Value.Item1, out var stack_value);
                        switch (kvp.Value.Item1.type) {
                            case VariableType.Integer:
                                ty = "int";
                                val = stack_value.ToString();
                                break;
                            case VariableType.Floating:
                                ty = "float";
                                val = CalcExpr.Utility.convert_float_from((uint)stack_value).ToString();
                                break;
                            case VariableType.Boolean:
                                ty = "bool";
                                val = stack_value != 0 ? "true" : "false";
                                break;
                        }
                        GUILayout.Label($"{kvp.Key}: {ty} = {val}");
                    }
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void on_inspector_gui_inner() {

        }

        private void clear_stack() {
            m_stack_count = 0;
            m_variable_dict.Clear();
        }

        private void push_stack_frame(IReadOnlyList<Variable> frame) {
            if (frame != null) {
                m_stack_count += frame.Count;
                for (int i = 0; i < frame.Count; ++i) {
                    var v = frame[i];
                    if (string.IsNullOrEmpty(v.name) || m_variable_dict.ContainsKey(v.name)) {
                        continue;
                    }
                    m_variable_dict.Add(v.name, (v, m_stack_count - i - 1));
                }
            }
        }
        
        public IReadOnlyDictionary<string, (Variable, int)> variable_dict => m_variable_dict;

        protected Dictionary<string, (Variable, int)> m_variable_dict = new Dictionary<string, (Variable, int)>();
        protected int m_stack_count;

        private static bool s_variable_dict_visible = true;

        public bool try_get_element_index(string name, out int index) {
            var node = m_node as ExpressionContextNode;
            if (node.referenced_elements != null) {
                for (int i = 0; i < node.referenced_elements.Length; ++i) {
                    if (name == $"_{i}") {
                        index = i;
                        return true;
                    }
                }
            }

            index = -1;
            return false;
        }

        public override object runtime_build_data(IContext context) {
            var data = new Dictionary<Variable, int>();
            foreach (var kvp in m_variable_dict) {
                var (v, p) = kvp.Value;
                data[v] = context.get_stack_int(p);
            }
            return data;
        }

        public override void runtime_enter(object data) {
            m_runtime_data = data as Dictionary<Variable, int>;
        }

        public override void runtime_leave() {
            m_runtime_data = null;
        }

        private Dictionary<Variable, int> m_runtime_data = null;
    }

    [NodeEditor(typeof(ExpressionContextNodeWithInput))]
    public class ExpressionContextNodeWithInputEditor : ExpressionContextNodeEditor {

        public override PortView get_input_port() {
            return view.static_input_ports[0];
        }

    }

}
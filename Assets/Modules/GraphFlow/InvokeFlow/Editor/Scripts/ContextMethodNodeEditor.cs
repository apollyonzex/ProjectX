
using GraphNode;
using GraphNode.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(ContextMethodNode))]
    public class ContextMethodNodeEditor : ExpressionContextNodeWithInputEditor {

        public ContextMethodNode this_node => (ContextMethodNode)node;

        public override void on_view_init() {
            m_port_view = view.find_output_port(new NodePropertyPort_InvokeWithVariables(NodePort.IO.Output, typeof(ContextMethodNode).GetProperty("output"), false));

            var node = this_node;
            m_methods = get_methods(node.target_type);
            if (node.method_info != null) {
                if (m_methods != null) {
                    for (int i = 0; i < m_methods.methods.Length; ++i) {
                        var info = m_methods.methods[i];
                        if (info.mi == node.method_info) {
                            m_index = i + 1;
                            m_info = info;
                            break;
                        }
                    }
                }
            }
            build_data();
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is ContextMethodNode other) {
                var self = this_node;
                other.method_info = self.method_info;
                if (self.parameters != null) {
                    other.parameters = new ContextMethodNode.Parameter[self.parameters.Length];
                    for (int i = 0; i < self.parameters.Length; ++i) {
                        ref var p = ref self.parameters[i];
                        other.parameters[i] = new ContextMethodNode.Parameter {
                            expression = new Expression { content = p.expression.content },
                            index = p.index,
                            type = p.type,
                        };
                    }
                }
                if (self.returns != null) {
                    other.returns = (ContextMethodNode.Return[])self.returns.Clone();
                }
            }
        }

        public override void on_body_gui() {
            base.on_body_gui();
            if (m_info == null) {
                GUILayout.Label("<None>");
            } else {
                bool error = false;
                foreach (var e in m_param_editors) {
                    if (e.err_msg != null) {
                        error = true;
                        break;
                    }
                }
                if (error) {
                    GUILayout.Label("Error", GraphResources.styles.red_label);
                } else {
                    GUILayout.Label(m_methods.names[m_index]);
                }
            }
        }

        protected override IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            if (m_port_view.port == iv_port) {
                return enumerate_returns();
            }
            return null;
        }

        public override void on_inspector_enable() {
            base.on_inspector_enable();
            foreach (var e in m_param_editors) {
                e.on_inspector_enable();
            }
        }

        public override void on_inspector_disable() {
            base.on_inspector_disable();
            foreach (var e in m_param_editors) {
                e.on_inspector_disable();
            }
        }

        protected override void on_inspector_gui_inner() {
            if (m_methods == null) {
                EditorGUILayout.HelpBox("Invalid context type", MessageType.Error);
            } else {
                var index = EditorGUILayout.Popup("Method", m_index, m_methods.names);
                if (index != m_index) {
                    var node = this_node;
                    var old_value = (node.parameters, m_index, m_param_editors.ToArray());
                    m_index = index;
                    if (m_index == 0) {
                        m_info = null;
                    } else {
                        m_info = m_methods.methods[m_index - 1];
                    }
                    build_data();
                    var new_value = (node.parameters, m_index, m_param_editors.ToArray());
                    view.graph.undo.record(new ChangeMethod { editor = this, old_value = old_value, new_value = new_value });
                    build_port_stack_frame(m_port_view, true);
                    view.size_changed = true;
                }
            }
            foreach (var e in m_param_editors) {
                e.on_inspector_gui();
            }
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            foreach (var e in m_param_editors) {
                e.build();
            }
        }

        private ContextMethods m_methods;
        private int m_index = 0;
        private ContextMethodInfo m_info = null;

        private IEnumerator<Variable> enumerate_returns() {
            if (m_info != null) {
                foreach (var r in m_info.returns) {
                    switch (r.type) {
                        case VariableType.Integer:
                            yield return new VariableInt(r.name);
                            break;
                        case VariableType.Floating:
                            yield return new VariableFloat(r.name);
                            break;
                        case VariableType.Boolean:
                            yield return new VariableBool(r.name);
                            break;
                    }
                }
            }
        }

        private void build_data() {
            m_param_editors.Clear();
            var node = this_node;
            if (m_info != null) {
                node.method_info = m_info.mi;
                node.parameter_count = m_info.parameter_count;
                var parameters = new ContextMethodNode.Parameter[m_info.parameters.Length];
                for (int i = 0; i < parameters.Length; ++i) {
                    ref var p = ref parameters[i];
                    ref var info = ref m_info.parameters[i];
                    p.index = info.index;
                    p.type = info.type;
                    p.expression = new Expression();
                    var ee = new ExpressionEditor();
                    ee.excepted_type = (CalcExpr.ValueType)info.type;
                    ee.attach(p.expression, null, view.graph.editor, this, $"{info.name}: {info.type.to_string()}");
                    m_param_editors.Add(ee);
                }
                if (node.parameters != null) {
                    var length = Mathf.Min(node.parameters.Length, parameters.Length);
                    for (int i = 0; i < length; ++i) {
                        parameters[i].expression.content = node.parameters[i].expression.content;
                    }
                }
                node.parameters = parameters;

                node.returns = new ContextMethodNode.Return[m_info.returns.Length];
                for (int i = 0; i < node.returns.Length; ++i) {
                    ref var r = ref node.returns[i];
                    ref var info = ref m_info.returns[i];
                    r.index = info.index;
                    r.type = info.type;
                }

                if (!stack_changed) {
                    foreach (var ee in m_param_editors) {
                        ee.build();
                    }
                }

            } else {
                node.method_info = null;
                node.parameter_count = 0;
                node.parameters = null;
                node.returns = null;
            }
        }

        private OutputPortView m_port_view;
        private List<ExpressionEditor> m_param_editors = new List<ExpressionEditor>();

        private class ChangeMethod : GraphUndo.ChangeValue<(ContextMethodNode.Parameter[] parameters, int index, ExpressionEditor[] param_editors)> {
            public ContextMethodNodeEditor editor;

            protected override void set_value(ref (ContextMethodNode.Parameter[] parameters, int index, ExpressionEditor[] param_editors) old_value, ref (ContextMethodNode.Parameter[] parameters, int index, ExpressionEditor[] param_editors) value) {
                editor.m_index = value.index;
                var node = editor.this_node;
                node.parameters = value.parameters;
                editor.m_param_editors.Clear();
                foreach (var ee in value.param_editors) {
                    editor.m_param_editors.Add(ee);
                }
                if (value.index == 0) {
                    editor.m_info = null;
                    node.method_info = null;
                    node.parameter_count = 0;
                    node.returns = null;
                } else {
                    editor.m_info = editor.m_methods.methods[value.index - 1];
                    node.method_info = editor.m_info.mi;
                    node.parameter_count = editor.m_info.parameter_count;
                    node.returns = new ContextMethodNode.Return[editor.m_info.returns.Length];
                    for (int i = 0; i < node.returns.Length; ++i) {
                        ref var r = ref node.returns[i];
                        ref var info = ref editor.m_info.returns[i];
                        r.index = info.index;
                        r.type = info.type;
                    }
                }
                editor.build_port_stack_frame(editor.m_port_view, true);
                editor.view.size_changed = true;
            }
        }

        static ContextMethods get_methods(Type type) {
            if (type == null) {
                return null;
            }
            if (!s_context_types.TryGetValue(type, out var methods)) {
                methods = build(type);
                s_context_types.Add(type, methods);
            }
            return methods;
        }


        class ContextMethodInfo {
            public MethodInfo mi;
            public int parameter_count;
            public (string name, VariableType type, int index)[] parameters;
            public (string name, VariableType type, int index)[] returns;
        }

        class ContextMethods {
            public string[] names;
            public ContextMethodInfo[] methods;
        }

        static Dictionary<Type, ContextMethods> s_context_types = new Dictionary<Type, ContextMethods>();

        static ContextMethods build(Type type) {
            var info = new List<ContextMethodInfo>();
            var parameters = new List<(string name, VariableType type, int index)>();
            var returns = new List<(string name, VariableType type, int index)>();
            var int_type = typeof(int);
            var int_ref_type = typeof(int).MakeByRefType();
            var float_type = typeof(float);
            var float_ref_type = typeof(float).MakeByRefType();
            var bool_type = typeof(bool);
            var bool_ref_type = typeof(bool).MakeByRefType();
            foreach (var mi in type.GetMethods()) {
                if (mi.Name.Length <= 3 || !mi.Name.StartsWith("cx_", StringComparison.Ordinal)) {
                    continue;
                }
                if (mi.ReturnType != typeof(void)) {
                    continue;
                }
                var ps = mi.GetParameters();
                bool failed = false;
                foreach (var pi in ps) {
                    if (pi.ParameterType != int_type && pi.ParameterType != float_type && pi.ParameterType != bool_type
                        && pi.ParameterType != int_ref_type && pi.ParameterType != float_ref_type && pi.ParameterType != bool_ref_type)
                    {
                        failed = true;
                        break;
                    }
                }
                if (failed) {
                    continue;
                }
                for (int i = 0; i < ps.Length; ++i) {
                    var pi = ps[i];
                    if (pi.ParameterType == int_type) {
                        parameters.Add((pi.Name, VariableType.Integer, i));
                    } else if (pi.ParameterType == int_ref_type) {
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            parameters.Add((pi.Name, VariableType.Integer, i));
                        }
                        returns.Add((pi.Name, VariableType.Integer, i));
                    } else if (pi.ParameterType == float_type) {
                        parameters.Add((pi.Name, VariableType.Floating, i));
                    } else if (pi.ParameterType == float_ref_type) {
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            parameters.Add((pi.Name, VariableType.Floating, i));
                        }
                        returns.Add((pi.Name, VariableType.Floating, i));
                    } else if (pi.ParameterType == bool_type) {
                        parameters.Add((pi.Name, VariableType.Boolean, i));
                    } else {
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            parameters.Add((pi.Name, VariableType.Boolean, i));
                        }
                        returns.Add((pi.Name, VariableType.Boolean, i));
                    }
                }
                info.Add(new ContextMethodInfo {
                    mi = mi,
                    parameter_count = ps.Length,
                    parameters = parameters.ToArray(),
                    returns = returns.ToArray(),
                });
                parameters.Clear();
                returns.Clear();
            }
            var ret = new ContextMethods {
                names = new string[info.Count + 1],
            };
            ret.names[0] = "<None>";
            for (int i = 0; i < info.Count; ++i) {
                ret.names[i + 1] = info[i].mi.Name.Substring(3);
            }
            ret.methods = info.ToArray();
            return ret;
        }
    }

}
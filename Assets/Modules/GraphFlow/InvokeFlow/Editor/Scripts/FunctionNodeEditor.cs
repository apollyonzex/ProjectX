
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(FunctionNode))]
    public class FunctionNodeEditor : ExpressionContextNodeWithInputEditor {

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("method", out var pe)) {
                pe.on_changed += on_method_changed;
                m_method_editor = pe as FunctionEditor;
            }
        }

        private void on_method_changed(PropertyEditor pe, bool by_user) {
            build_port_stack_frame(m_port_view, true);
        }

        public override void on_view_init() {
            m_port_view = view.find_output_port(new NodePropertyPort_InvokeWithVariables(NodePort.IO.Output, typeof(FunctionNode).GetProperty("output"), false));
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            m_method_editor.build_parameters();
        }

        protected override IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            if (m_port_view.port == iv_port) {
                return enumerate_returns();
            }
            return null;
        }

        private IEnumerator<Variable> enumerate_returns() {
            var node = this.node as FunctionNode;
            if (node.method != null && node.method.method != null) {
                for (int i = 0; i < node.method.out_names.Length; ++i) {
                    var name = node.method.out_names[i];
                    if (string.IsNullOrEmpty(name)) {
                        continue;
                    }
                    switch (node.method.method.parameter_types[node.method.method.output_indices[i]]) {
                        case FunctionParameterType.IntegerRef:
                            yield return new VariableInt(name);
                            break;
                        case FunctionParameterType.FloatingRef:
                            yield return new VariableFloat(name);
                            break;
                        case FunctionParameterType.BooleanRef:
                            yield return new VariableBool(name);
                            break;
                    }
                }
            }
        }

        private OutputPortView m_port_view;
        private FunctionEditor m_method_editor;
    }

    public class FunctionNodeEditor<T> : ExpressionContextNodeWithInputEditor {

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("method", out var pe)) {
                pe.on_changed += on_method_changed;
                m_method_editor = pe as FunctionEditor<T>;
            }
        }

        private void on_method_changed(PropertyEditor pe, bool by_user) {
            build_port_stack_frame(m_port_view, true);
        }

        public override void on_view_init() {
            m_port_view = view.find_output_port(new NodePropertyPort_InvokeWithVariables(NodePort.IO.Output, typeof(FunctionNode<T>).GetProperty("output"), false));
        }

        protected override IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            if (m_port_view.port == iv_port) {
                return enumerate_returns();
            }
            return null;
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            m_method_editor.build_parameters();
        }

        private IEnumerator<Variable> enumerate_returns() {
            var node = this.node as FunctionNode<T>;
            if (node.method != null && node.method.method != null) {
                for (int i = 0; i < node.method.out_names.Length; ++i) {
                    var name = node.method.out_names[i];
                    if (string.IsNullOrEmpty(name)) {
                        continue;
                    }
                    switch (node.method.method.parameter_types[node.method.method.output_indices[i]]) {
                        case FunctionParameterType.IntegerRef:
                            yield return new VariableInt(name);
                            break;
                        case FunctionParameterType.FloatingRef:
                            yield return new VariableFloat(name);
                            break;
                        case FunctionParameterType.BooleanRef:
                            yield return new VariableBool(name);
                            break;
                    }
                }
            }
        }

        private FunctionEditor<T> m_method_editor;
        private OutputPortView m_port_view;
    }
}
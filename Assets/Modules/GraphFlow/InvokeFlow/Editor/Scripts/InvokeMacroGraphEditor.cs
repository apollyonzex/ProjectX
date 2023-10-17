
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [GraphEditor(typeof(InvokeMacroGraph))]
    public class InvokeMacroGraphEditor : InvokeGraphEditor {

        public override List<Variable> stack_frame => m_stack_frame;

        public override void attach(Graph graph, GraphView view) {
            base.attach(graph, view);
            if (graph is InvokeMacroGraph g) {
                if (g.external_elements == null) {
                    g.external_elements = new List<ExternalElementNode>();
                }
                if (g.external_collections == null) {
                    g.external_collections = new List<ExternalCollectonNode>();
                }
            }
        }

        public override void build_stack_frame(bool rise_event) {
            m_stack_frame.Clear();

            if (graph is InvokeMacroGraph g) {

                g.argument_count = m_inputs_editor.available_variables.Count;
                g.return_count = m_outputs_editor.available_variables.Count;
                g.variable_count = m_variables_editor.available_variables.Count;
                
                foreach (var item in m_outputs_editor.available_variables) { 
                    m_stack_frame.Add(item);
                }

                foreach (var item in m_variables_editor.available_variables) {
                    m_stack_frame.Add(item);
                }

                foreach (var item in m_inputs_editor.available_variables) {
                    m_stack_frame.Add(item);
                }

                g.stack_frame = new int[g.return_count + g.variable_count];
                for (int i = 0; i < g.stack_frame.Length; ++i) {
                    g.stack_frame[i] = m_stack_frame[i].value_in_stack;
                }
            }

            if (rise_event) {
                foreach (var node in view.nodes) {
                    if (node.editor is InvokeNodeEditor ne) {
                        ne.notify_stack_changed();
                    }
                }
            }
        }

        protected override void init_property_editors() {
            base.init_property_editors();
            if (try_get_property("outputs", out var pe)) {
                m_outputs_editor = pe as VariablesEditor;
            }
            if (try_get_property("inputs", out pe)) {
                m_inputs_editor = pe as ParametersEditor;
            }
        }

        List<Variable> m_stack_frame = new List<Variable>();

        protected VariablesEditor m_outputs_editor;
        protected ParametersEditor m_inputs_editor;
    }

}
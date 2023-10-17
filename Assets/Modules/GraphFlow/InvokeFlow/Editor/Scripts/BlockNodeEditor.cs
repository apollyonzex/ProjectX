
using System.Collections.Generic;
using GraphNode;
using GraphNode.Editor;

using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(BlockNode))]
    public class BlockNodeEditor : InvokeNodeWithInputEditor {

        public new BlockNode node => (BlockNode)m_node;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("variables", out var pe)) {
                m_variables_editor = pe as VariablesEditor;
            }
        }

        public override void build_stack_frame(bool rise_event) {
            var node = this.node;
            
            
            node.stack_frame = new int[m_variables_editor.available_variables.Count];
            for (int i = 0; i < node.stack_frame.Length; ++i) {
                node.stack_frame[i] = m_variables_editor.available_variables[i].value_in_stack;
            }

            if (rise_event) {
                notify_stack_changed();
            }
        }

        public override IReadOnlyList<Variable> get_stack_frame() {
            return m_variables_editor.available_variables;
        }

        public override void on_body_gui() {
            base.on_body_gui();
            foreach (var item in m_variables_editor.available_variables) {
                GUILayout.Label(VariablesEditor.get_variable_display(item));
            }
        }

        VariablesEditor m_variables_editor;
    }

}
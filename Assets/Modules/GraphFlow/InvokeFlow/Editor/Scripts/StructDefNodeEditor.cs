
using GraphNode;
using GraphNode.Editor;

using System.Collections.Generic;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(StructDefNode))]
    public class StructDefNodeEditor : InvokeNodeEditor, INamedNodeEditor<StructDefNodeEditor> {

        public int name_index { get; set; }

        public event System.Action<StructDefNodeEditor> on_removed;

        public event System.Action on_members_changed;

        public new StructDefNode node => (StructDefNode)m_node;

        public List<Variable> available_members => m_variables_editor.available_variables;

        string INamedNodeEditor<StructDefNodeEditor>.name {
            get {
                var node = this.node;
                return $"{node.name}<{node.GetHashCode():X}>";
            }
        }

        public override Color node_color => new Color32(94, 90, 55, 255);

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("members", out var pe)) {
                m_variables_editor = pe as VariablesEditor;
            }
            if (try_get_property("name", out pe)) {
                pe.on_changed += on_name_changed;
            }
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.struct_defs.add(this, false);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.struct_defs.add(this, true);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            on_removed?.Invoke(this);
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.struct_defs.remove(this);
            }
        }

        public override void build_stack_frame(bool rise_event) {
            var node = this.node;
            if (m_variables_editor.available_variables.Count != 0) {
                node.stack_frame = new int[m_variables_editor.available_variables.Count];
                for (int i = 0; i < node.stack_frame.Length; ++i) {
                    node.stack_frame[i] = m_variables_editor.available_variables[i].value_in_stack;
                }
            } else {
                node.stack_frame = null;
            }
   

            if (rise_event) {
                on_members_changed?.Invoke();
            }
        }

        private void on_name_changed(PropertyEditor _, bool by_user) {
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.struct_defs.notify_name_changed(this);
            }
        }

        VariablesEditor m_variables_editor;
    }
}
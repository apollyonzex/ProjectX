

using GraphNode.Editor;
using System;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(ProcEntryNode))]
    public class ProcEntryNodeEditor : InvokeNodeEditor, INamedNodeEditor<ProcEntryNodeEditor> {

        public int name_index { get; set; } = 0;

        string INamedNodeEditor<ProcEntryNodeEditor>.name {
            get {
                var node = this.node;
                return $"{node.name}<{node.GetHashCode():X}>";
            }
        }

        public new ProcEntryNode node => (ProcEntryNode)m_node;

        public override sealed bool can_access_graph_variables => false;

        public override void on_view_init() {
            base.on_view_init();
            //var node = this_node;

            if (try_get_property("name", out var pe)) {
                pe.on_changed += on_name_changed;
            }

        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.proc_entries.add(this, false);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.proc_entries.add(this, true);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            on_removed?.Invoke(this);
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.proc_entries.remove(this);
            }
        }

        public override void build_stack_frame(bool rise_event) {
            var node = this.node;
            m_stack_frame.Clear();
            var iter = node.returns.enumerate_valid_variables();
            while (iter.MoveNext()) {
                m_stack_frame.Add(iter.Current);
            }
            node.stack_frame_returns = new int[m_stack_frame.Count];
            for (int i = 0; i < node.stack_frame_returns.Length; ++i) {
                node.stack_frame_returns[i] = m_stack_frame[i].value_in_stack;
            }

            iter = node.arguments.enumerate_valid_variables();
            while (iter.MoveNext()) {
                m_stack_frame.Add(iter.Current);
            }

            on_build_stack_done?.Invoke();

            if (rise_event) {
                notify_stack_changed();
            }
        }

        public override IReadOnlyList<Variable> get_stack_frame() => m_stack_frame;

        public event Action on_build_stack_done;
        public event Action<ProcEntryNodeEditor> on_removed;


        private void on_name_changed(PropertyEditor _, bool by_user) {
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.proc_entries.notify_name_changed(this);
            }
        }

        protected List<Variable> m_stack_frame = new List<Variable>();
    }

}
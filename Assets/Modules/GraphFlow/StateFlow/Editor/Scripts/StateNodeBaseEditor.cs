
using GraphNode.Editor;
using InvokeFlow.Editor;

namespace StateFlow.Editor {

    [NodeEditor(typeof(StateNodeBase))]
    public class StateNodeBaseEditor : InvokeNodeEditor {

        public new StateNodeBase node => m_node as StateNodeBase;

        public int name_index = 0;

        public event System.Action on_removed;

        public override void on_graph_open() {
            base.on_graph_open();
            var node = this.node;
            if (node.name != null) {
                if (view.graph.editor is StateGraphEditor ge) {
                    ge.state_dict.Add(node.name, this);
                }
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            var node = this.node;
            if (node.name != null) {
                if (view.graph.editor is StateGraphEditor ge) {
                    ge.notify_state_added(this);
                }
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            name_index = 0;
            on_removed?.Invoke();
            var node = this.node;
            if (node.name != null) {
                if (view.graph.editor is StateGraphEditor ge) {
                    ge.notify_state_removed(node.name);
                }
            }
        }

    }
}
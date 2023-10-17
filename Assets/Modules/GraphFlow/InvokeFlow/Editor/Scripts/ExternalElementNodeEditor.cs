
using GraphNode.Editor;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(ExternalElementNode))]
    public class ExternalElementNodeEditor : ElementNodeEditor {

        public new ExternalElementNode node => m_node as ExternalElementNode;

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor.graph is InvokeMacroGraph g) {
                g.external_elements.Add(node);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor.graph is InvokeMacroGraph g) {
                g.external_elements.Remove(node);
            }
        }
    }
}
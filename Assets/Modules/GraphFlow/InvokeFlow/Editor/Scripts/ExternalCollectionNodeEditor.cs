
using GraphNode.Editor;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(ExternalCollectonNode))]
    public class ExternalCollectionNodeEditor : CollectionNodeBaseEditor {

        public new ExternalCollectonNode node => (ExternalCollectonNode)m_node;

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor.graph is InvokeMacroGraph g) {
                g.external_collections.Add(node);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor.graph is InvokeMacroGraph g) {
                g.external_collections.Remove(node);
            }
        }

    }
}
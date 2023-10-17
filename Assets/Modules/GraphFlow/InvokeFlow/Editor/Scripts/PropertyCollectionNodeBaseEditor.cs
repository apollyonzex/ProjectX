
using GraphNode.Editor;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(CollectionNodeBase))]
    public class PropertyCollectionNodeBaseEditor : PropertyNamedNodeEditor<CollectionNodeBaseEditor> {
        public override NodeEditorList<CollectionNodeBaseEditor> node_editor_list => (m_graph as InvokeGraphEditor).collections;

        protected override string get_display_in_body() {
            if (m_value_editor == null) {
                return "Empty";
            }
            return $"'{m_value_editor.node.name}'";
        }
    }
}
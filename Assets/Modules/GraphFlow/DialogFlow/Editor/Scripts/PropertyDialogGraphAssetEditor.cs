
using GraphNode.Editor;

namespace DialogFlow.Editor {

    [PropertyEditor(typeof(DialogGraphAsset))]
    public class PropertyDialogGraphAssetEditor : PropertyUnityObjectEditor<DialogGraphAsset> {

        protected override void validate(ref DialogGraphAsset new_value) {
            if (new_value == null) {
                return;
            }
            if (m_graph.view.window?.asset == new_value) {
                new_value = null;
            } else if (m_graph.graph is DialogGraph graph) {
                var target_graph = new_value.graph_unchecked ?? (DialogGraph)new_value.new_graph();
                if (!target_graph.context_type.IsAssignableFrom(graph.context_type)) {
                    new_value = null;
                }
            }
        }
    }
}
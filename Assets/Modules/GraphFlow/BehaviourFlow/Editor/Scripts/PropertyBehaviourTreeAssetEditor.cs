
using GraphNode.Editor;

namespace BehaviourFlow.Editor {

    [PropertyEditor(typeof(BehaviourTreeAsset))]
    public class PropertyBehaviourTreeAssetEditor : PropertyUnityObjectEditor<BehaviourTreeAsset> {

        protected override void validate(ref BehaviourTreeAsset new_value) {
            if (new_value == null) {
                return;
            }
            if (m_graph.view.window?.asset == new_value) {
                new_value = null;
            } else if (m_graph.graph is BehaviourTree graph) {
                var target_graph = new_value.graph_unchecked ?? (BehaviourTree)new_value.new_graph();
                if (!target_graph.context_type.IsAssignableFrom(graph.context_type)) {
                    new_value = null;
                }
            }
        }
    }
}
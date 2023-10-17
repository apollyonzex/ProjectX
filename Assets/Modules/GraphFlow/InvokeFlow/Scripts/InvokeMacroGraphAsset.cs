
using GraphNode;
using UnityEngine;

namespace InvokeFlow {

    [CreateAssetMenu(menuName = "Graph/Generic/InvokeMacro", fileName = "invoke_macro")]
    public class InvokeMacroGraphAsset : GraphAsset<InvokeMacroGraph> {

        public override Graph new_graph() {
            return init_macro_graph(new InvokeMacroGraph());
        }

        public static InvokeMacroGraph init_macro_graph(InvokeMacroGraph graph) {
            graph.entry = new EntryNode();
            graph.nodes = new Node[] { graph.entry };
            return graph;
        }
    }

}
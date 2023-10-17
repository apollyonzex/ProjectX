
using UnityEngine;
using GraphNode;

namespace DialogFlow {

    [CreateAssetMenu(fileName = "dialog", menuName = "Graph/Generic/Dialog")]
    public class DialogGraphAsset : GraphAsset<DialogGraph> {
        public override Graph new_graph() {
            return init_graph(new DialogGraph());
        }

        public static DialogGraph init_graph(DialogGraph graph) {
            graph.entry = new EntryNode();
            graph.nodes = new Node[] { graph.entry };
            return graph;
        }
    }

    public class DialogGraphAsset<T> : DialogGraphAsset where T : DialogGraph, new() {

        public override Graph new_graph() {
            return init_graph(new T());
        }
    }

}
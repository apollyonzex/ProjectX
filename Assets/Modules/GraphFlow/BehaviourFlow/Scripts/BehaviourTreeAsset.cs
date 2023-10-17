
using UnityEngine;
using GraphNode;

namespace BehaviourFlow {

    [CreateAssetMenu(fileName = "bt_generic", menuName = "Graph/Generic/BehaviourTree")]
    public class BehaviourTreeAsset : GraphAsset<BehaviourTree> {

        public override Graph new_graph() {
            return init_graph(new BehaviourTree());
        }

        public static BehaviourTree init_graph(BehaviourTree graph) {
            graph.entry = new Nodes.EntryNode();
            graph.nodes = new Node[] { graph.entry };
            return graph;
        }

    }

    public class BehaviourTreeAsset<T> : BehaviourTreeAsset where T : BehaviourTree, new() {
        public override Graph new_graph() {
            return init_graph(new T());
        }
    }
}
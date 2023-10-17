
using GraphNode;
using InvokeFlow;
using UnityEngine;

namespace StateFlow {

    [CreateAssetMenu(fileName = "state_graph", menuName = "Graph/Generic/StateGraph")]
    public class StateGraphAsset : InvokeGraphAsset {

        public override Graph new_graph() {
            return init_graph(new StateGraph());
        }
        public static Graph init_graph(StateGraph graph) {
            graph.entry = new EntryStateNode();
            graph.nodes = new Node[] { graph.entry };
            return graph;
        }

        public new StateGraph graph => get_graph<StateGraph>();
        public new StateGraph graph_unchecked => get_graph_unchecked<StateGraph>();
    }

    public class StateGraphAsset<T> : StateGraphAsset where T : StateGraph, new() {

        public override Graph new_graph() {
            return init_graph(new T());
        }

        public new T graph => get_graph<T>();
        public new T graph_unchecked => get_graph_unchecked<T>();
    }
}
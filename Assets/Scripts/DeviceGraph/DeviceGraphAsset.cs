
using GraphNode;
using UnityEngine;

namespace DeviceGraph {

    [CreateAssetMenu()]
    public class DeviceGraphAsset : GraphAsset<DeviceGraph> {
        public override Graph new_graph() {
            return init_graph(new DeviceGraph());
        }

        public static DeviceGraph init_graph(DeviceGraph graph) {
            graph.nodes = new Node[] {  };
            return graph;
        }
    }
}

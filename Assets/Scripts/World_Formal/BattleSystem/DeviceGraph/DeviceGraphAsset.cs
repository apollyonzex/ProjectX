
using GraphNode;
using UnityEditor;
using UnityEngine;

namespace World_Formal.BattleSystem.DeviceGraph
{

    [CreateAssetMenu(fileName = "new FDeviceGraphAsset" ,menuName = "Fromal/DeviceGraphAsset")]
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

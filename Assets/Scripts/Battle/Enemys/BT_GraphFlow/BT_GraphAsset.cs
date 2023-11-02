using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow
{
    [CreateAssetMenu(fileName = "bt_graph", menuName = "DIY_Graph/BT_Graph")]
    public class BT_GraphAsset : GraphAsset<BT_Graph>
    {
        public override Graph new_graph()
        {
            return init_graph(new BT_Graph());
        }


        public static BT_Graph init_graph(BT_Graph graph)
        {
            graph.nodes = new Node[] { };
            return graph;
        }
    }
}


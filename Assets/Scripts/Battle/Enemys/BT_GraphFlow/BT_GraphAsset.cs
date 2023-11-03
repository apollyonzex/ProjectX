using Common;
using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow
{
    [CreateAssetMenu(fileName = "bt_graph", menuName = "DIY_Graph/BT_Graph")]
    public class BT_GraphAsset : GraphAsset<BT_Graph>
    {

        //==================================================================================================

        public override Graph new_graph()
        {
            return init_graph(new BT_Graph());
        }


        public static BT_Graph init_graph(BT_Graph graph)
        {
            graph.nodes = new Node[] { };
            return graph;
        }


        public override bool save_graph(Graph graph)
        {
            var bl = base.save_graph(graph);

            Mission.instance.try_get_mgr(Config.EnemyMgr_Name, out EnemyMgr mgr);
            if (mgr == null) return bl;

            foreach (var (_,cell) in mgr.cell_dic)
            {
                attach(cell.bctx, graph);
            }

            return bl;
        }


        public void attach(BT_Context bctx, Graph graph)
        {
            if (bctx.graph_name == name && graph is BT_Graph _graph)
            {
                bctx.start_ac = _graph.start_ac;
                bctx.cpns_dic = _graph.cpns_dic;
            }
        }
    }
}


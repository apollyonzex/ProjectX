using Battle.Enemys.BT_GraphFlow.Nodes;
using GraphNode;
using System;
using System.Collections.Generic;

namespace Battle.Enemys.BT_GraphFlow
{
    public class BT_Context : IContext
    {
        public Enum.EN_ret_state ret = Enum.EN_ret_state.none;
        public Stack<BT_Decide_Node> decide_nodes = new();

        public System.Action<BT_Context> start_ac;
        public Dictionary<string, BT_CPN> cpns_dic = new();

        public string graph_name;

        Type IContext.context_type => typeof(BT_Context);

        //================================================================================================

        public BT_Context(BT_GraphAsset asset)
        {
            graph_name = asset.name;
            attach(asset);
        }


        public void attach(BT_GraphAsset asset)
        {
            if (graph_name == asset.name && asset.graph is BT_Graph _graph)
            {
                cpns_dic.Clear();

                foreach (var node in _graph.nodes)
                {
                    if (node is StartNode sn)
                    {
                        start_ac = sn.do_start;
                        continue;
                    }

                    if (node is BT_DSNode dn)
                    {
                        var cpn = dn.init_cpn<BT_CPN>();
                        cpn.init(this, dn);
                        cpns_dic.Add(dn.module_name, cpn);
                    }
                }
            }
        }


        public bool try_do_back()
        {
            if (decide_nodes.TryPeek(out var decide_node))
            {
                decide_node.do_back(this);
                return true;
            }

            return false;
        }


        public void tick()
        {
            start_ac?.Invoke(this);
        }
    }
}


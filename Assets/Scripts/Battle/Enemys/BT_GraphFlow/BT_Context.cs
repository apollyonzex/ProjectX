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
        public Dictionary<string, BT_CPN> cpns_dic;

        public string graph_name;

        Type IContext.context_type => typeof(BT_Context);

        //================================================================================================

        public BT_Context(BT_GraphAsset asset)
        {
            graph_name = asset.name;
            asset.attach(this, asset.graph);
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


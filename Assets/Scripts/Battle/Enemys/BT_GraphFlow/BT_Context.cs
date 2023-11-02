using GraphNode;
using System;
using System.Collections.Generic;

namespace Battle.Enemys.BT_GraphFlow
{
    public class BT_Context : IContext
    {
        public Enum.EN_ret_state ret = Enum.EN_ret_state.none;
        public Stack<BT_Decide_Node> decide_nodes = new();

        Type IContext.context_type => typeof(BT_Context);

        System.Action<BT_Context> m_start;

        //================================================================================================

        public BT_Context(BT_GraphAsset asset)
        {
            m_start = asset.graph.start_ac;
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
            m_start?.Invoke(this);
        }
    }
}


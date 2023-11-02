using GraphNode;
using System;
using System.Collections.Generic;

namespace Battle.Enemys.BT_GraphFlow
{
    public class BT_Context : IContext
    {
        Type IContext.context_type => typeof(BT_Context);

        System.Action<BT_Context> m_start;

        public Enum.EN_ret_state ret = Enum.EN_ret_state.none;
        public Stack<BT_Decide_Node> decide_nodes = new();

        //================================================================================================

        public BT_Context(BT_GraphAsset asset)
        {
            var graph = asset.graph;

            foreach (var node in graph.nodes)
            {
                if (node is Nodes.StartNode _s)
                {
                    m_start = _s.do_start;
                    break;
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
            m_start?.Invoke(this);
        }
    }
}


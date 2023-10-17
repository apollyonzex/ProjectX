using GraphNode;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    [Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_CaravanNode : BT_DSNode
    {
        public override Type cpn_type => typeof(BT_Caravan);
    }
}


using GraphNode;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_ShareBoolNode : BT_DSNode
    {
        public override Type cpn_type => typeof(BT_ShareBool);

        [ShowInBody(format = "value -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression value;

        //================================================================================================



    }
}


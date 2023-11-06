using Battle.Enemys.BT_GraphFlow.CPNs;
using GraphNode;
using System;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_BoolNode : BT_DSNode
    {
        [ShowInBody(format = "value -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression value;

        public override Type cpn_type => typeof(BT_Bool);

        //================================================================================================
    }
}


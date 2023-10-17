using GraphNode;
using System;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_Random_FloatNode : BT_DSNode
    {
        public override Type cpn_type => typeof(BT_Random_Float);

        [ShowInBody(format = "min -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression min;

        [ShowInBody(format = "max -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression max;

        //================================================================================================



    }
}


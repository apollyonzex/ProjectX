using GraphNode;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_FloatNode : BT_DSNode
    {
        [ShowInBody(format = "bl -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression bl;

        //================================================================================================

        public override void init()
        {
            cpn = new CPNs.BT_Float();
        }
    }
}


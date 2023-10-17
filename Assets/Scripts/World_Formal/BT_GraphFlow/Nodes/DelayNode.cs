using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class DelayNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public BT_Expression time;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            time.calc(ctx, typeof(BT_Context), out int _time);
            ctx.freeze(_time);
        }
    }
}


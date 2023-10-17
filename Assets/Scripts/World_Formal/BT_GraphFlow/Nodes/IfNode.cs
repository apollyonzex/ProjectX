using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class IfNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression condition;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        #region Output
        [Output]
        [Display("true", seq = 1)]
        public void o_true(BT_Context ctx)
        {
            var bl = condition.do_calc_bool(ctx);

            if (!bl)
                ctx.unactive_nodes_stack.Push(this);
        }


        [Output]
        [Display("false", seq = 2)]
        public void o_false(BT_Context ctx)
        {
            var bl = condition.do_calc_bool(ctx);

            if (bl)
                ctx.unactive_nodes_stack.Push(this);
        }
        #endregion
    }
}


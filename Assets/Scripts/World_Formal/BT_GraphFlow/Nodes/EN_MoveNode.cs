using GraphNode;
using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class EN_MoveNode : BT_Node
    {
        [ShowInBody(format = "vy -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression vy;

        [ShowInBody(format = "vx -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression vx;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            var _vx = vx.do_calc_float(ctx);
            var _vy = vy.do_calc_float(ctx);

            Enemy_Move_Helper.instance.move(ctx, new Vector2(_vx, _vy));

            ctx.is_last_method_ret = true;
        }
    }
}


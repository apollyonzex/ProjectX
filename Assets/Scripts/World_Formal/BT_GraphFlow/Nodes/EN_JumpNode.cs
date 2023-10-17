using GraphNode;
using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class EN_JumpNode : BT_Node
    {
        [ShowInBody(format = "height -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression height;

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
            var _height = height.do_calc_float(ctx);

            Enemy_Move_Helper.instance.jump(ctx, _vx, _height);

            ctx.is_last_method_ret = true;
        }
    }
}


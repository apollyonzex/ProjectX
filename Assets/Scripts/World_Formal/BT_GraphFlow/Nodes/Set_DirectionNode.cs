using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Set_DirectionNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression x;

        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression y;

        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression is_upside_down; //是否启用倒转

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            var _x = x.do_calc_float(ctx);
            var _y = y.do_calc_float(ctx);

            ctx.direction = new(_x, _y);
            ctx.is_upside_down = is_upside_down.do_calc_bool(ctx);

            ctx.is_last_method_ret = true;
        }

        

        
    }
}


using GraphNode;
using System;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_Vector2Node : BT_DSNode
    {
        public override Type cpn_type => typeof(BT_Vector2);

        [ShowInBody(format = "x -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression x;

        [ShowInBody(format = "y -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression y;

        public bool is_use_prms;

        //================================================================================================

        #region Input
        [Input]
        [Display("in", seq = 1)]
        public System.Func<BT_Context, Vector2?> _i { get; set; }
        #endregion


        #region OutPut
        [Output]
        [Display("out", seq = 1)]
        public Vector2? _o(BT_Context ctx)
        {
            if (!is_use_prms)
            {
                return _i?.Invoke(ctx);
            }

            var _x = x.do_calc_float(ctx);
            var _y = y.do_calc_float(ctx);
            return new Vector2(_x, _y);
        }
        #endregion


        public Vector2 get_result(BT_Context ctx)
        {
            return (Vector2)_o(ctx);
        }

    }
}


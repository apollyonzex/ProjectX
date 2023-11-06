using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class IfNode : BT_Node
    {
        [ShowInBody(format = "bl -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public BT_Expression bl;

        //==================================================================================================

        #region Input
        [Input]
        [Display("in")]
        public void _i(BT_Context ctx)
        {
            var _bl = bl.do_calc_bool(ctx);

            if (_bl)
            {
                if (_o_true == null)
                {
                    ctx.ret = Enum.EN_ret_state.fail;
                    return;
                }

                _o_true.Invoke(ctx);
            }

            else
            {
                if (_o_false == null)
                {
                    ctx.ret = Enum.EN_ret_state.fail;
                    return;
                }

                _o_false.Invoke(ctx);
            }  
        }
        #endregion


        #region Output
        [Output]
        [Display("true")]
        public System.Action<BT_Context> _o_true { get; set; }


        [Output]
        [Display("false")]
        public System.Action<BT_Context> _o_false { get; set; }
        #endregion
    }
}


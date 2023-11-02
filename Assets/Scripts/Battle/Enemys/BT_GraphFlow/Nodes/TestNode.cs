using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class TestNode : BT_Node
    {
        [ShowInBody(format = "bl -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression bl;

        //==================================================================================================

        #region Input
        [Input]
        [Display("in")]
        public void _i(BT_Context ctx)
        {
            Debug.Log(comment);

            bl.calc(ctx, typeof(BT_Context), out bool _bl);
            if (_bl)
                ctx.ret = Enum.EN_ret_state.success;
            else
                ctx.ret = Enum.EN_ret_state.fail;

            ctx.try_do_back();
        }
        #endregion
    }
}


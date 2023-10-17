using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Epr_TestNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression _test;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        #region Output
        [Output]
        [Display("out", seq = 1)]
        public void o_true(BT_Context ctx)
        {
            do_self(ctx);
        }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            var num = _test.do_calc_float(ctx);
            Debug.Log(num);

            ctx.is_last_method_ret = true;
        }


        

    }
}



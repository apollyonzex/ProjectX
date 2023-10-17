using GraphNode;
using UnityEngine;
using World_Formal.BT_GraphFlow.Nodes.DS_Nodes;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_Set_ShareIntNode : BT_Node
    {
        [ShowInBody(format = "module_name -> {0}")]
        public string module_name;

        [ShowInBody(format = "value -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public BT_Expression value;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            if (!ctx.cpns_dic.TryGetValue(module_name, out var cpn)) return;
            if (cpn is not BT_ShareInt e) return;

            e.reset(value.do_calc_int(ctx));

            ctx.is_last_method_ret = true;
        }
    }
}



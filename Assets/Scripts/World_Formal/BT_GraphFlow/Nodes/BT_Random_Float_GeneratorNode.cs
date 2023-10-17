using GraphNode;
using UnityEngine;
using World_Formal.BT_GraphFlow.Nodes.DS_Nodes;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class BT_Random_Float_GeneratorNode : BT_Node
    {
        [ShowInBody(format = "module_name -> {0}")]
        public string module_name;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            if (!ctx.cpns_dic.TryGetValue(module_name, out var cpn)) return;
            if (cpn is not BT_Random_Float rf) return;

            rf.do_rnd();

            ctx.is_last_method_ret = true;
        }
    }
}



using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class LogNode : BT_Node
    {


        //================================================================================================

        #region Input
        [Display("_")]
        [Input]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        #region Output
        [Display("out")]
        [Output]
        public void o(BT_Context ctx)
        {
            do_self(ctx);
        }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            Debug.Log($"{comment}");

            ctx.is_last_method_ret = true;
        }
    }
}


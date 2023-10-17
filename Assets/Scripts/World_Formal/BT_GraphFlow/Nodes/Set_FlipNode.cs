using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Set_FlipNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        public Enemys.Enum.EN_Flip direction;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            ctx.flip = direction;

            ctx.is_last_method_ret = true;
        }

        

        
    }
}


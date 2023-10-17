using GraphNode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class EndNode : BT_Node
    {


        //================================================================================================

        #region Input
        [Display("end")]
        [Input]
        public System.Action<BT_Context> i_end { get; set; }


        public override void do_self(BT_Context ctx)
        {
        }
        #endregion
    }
}


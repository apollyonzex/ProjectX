using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class StartNode : BT_Node
    {
        #region Output
        [Output]
        [Display("out")]
        public System.Action<BT_Context> _o { get; set; }
        #endregion


        public void do_start(BT_Context ctx)
        {
            Debug.Log("start");

            ctx.ret = Enum.EN_ret_state.none;
            _o?.Invoke(ctx);
        }
    }
}


using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class ExecuteNodeNode : BT_Decide_Node
    {
        const int max_o_count = 5;

        //==================================================================================================

        #region Input
        [Input(can_multi_connect = true)]
        [Display("in")]
        public void _i(BT_Context ctx)
        {
            if (seq == 0) //首次进入
            {
                ctx.ret = Enum.EN_ret_state.none;
                ctx.decide_nodes.Push(this);
            }

            var ac = select_ac(++seq);

            if (seq > max_o_count || ac == null) //last检定：返回ret
            {
                ctx.ret = Enum.EN_ret_state.success;
                jump_out(ctx);
                return;
            }

            ac.Invoke(ctx);
        }
        #endregion


        #region Output
        [Output]
        [Display("1")]
        public System.Action<BT_Context> _o1 { get; set; }


        [Output]
        [Display("2")]
        public System.Action<BT_Context> _o2 { get; set; }


        [Output]
        [Display("3")]
        public System.Action<BT_Context> _o3 { get; set; }


        [Output]
        [Display("4")]
        public System.Action<BT_Context> _o4 { get; set; }


        [Output]
        [Display("5")]
        public System.Action<BT_Context> _o5 { get; set; }
        #endregion


        public override void do_back(BT_Context ctx)
        {
            _i(ctx);
        }

    }
}

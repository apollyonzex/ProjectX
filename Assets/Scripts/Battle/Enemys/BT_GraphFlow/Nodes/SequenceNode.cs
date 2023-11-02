using GraphNode;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class SequenceNode : BT_Decide_Node
    {
        public int seq;

        const int max_o_count = 5;

        //==================================================================================================

        #region Input
        [Input]
        [Display("in")]
        public void _i(BT_Context ctx)
        {
            if (seq == 0) //首次进入
            {
                ctx.ret = Enum.EN_ret_state.none;
                ctx.decide_nodes.Push(this);
            }

            if (ctx.ret == Enum.EN_ret_state.fail) return; //失败检定：子节点失败，则本节点失败

            var ac = select_ac(++seq);

            if (seq > max_o_count || ac == null) //last检定
            {
                seq = 0;

                ctx.ret = Enum.EN_ret_state.success;
                ctx.decide_nodes.Pop();

                ctx.try_do_back();

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


        System.Action<BT_Context> select_ac(int _seq)
        {
            var mi = GetType().GetMethod($"get__o{_seq}");
            return (System.Action<BT_Context>)mi?.Invoke(this, null);
        }


        public override void do_back(BT_Context ctx)
        {
            _i(ctx);
        }



    }
}

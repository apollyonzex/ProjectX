using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class SelectorNode : BT_Node
    {

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        #region Output
        [Output]
        [Display("1", seq = 1)]
        public void o1(BT_Context ctx)
        {
        }


        [Output]
        [Display("2", seq = 2)]
        public void o2(BT_Context ctx)
        {
            valid_result(ctx);
        }


        [Output]
        [Display("3", seq = 3)]
        public void o3(BT_Context ctx)
        {
            valid_result(ctx);
        }


        [Output]
        [Display("4", seq = 4)]
        public void o4(BT_Context ctx)
        {
            valid_result(ctx);
        }


        [Output]
        [Display("5", seq = 5)]
        public void o5(BT_Context ctx)
        {
            valid_result(ctx);
        }
        #endregion


        void valid_result(BT_Context ctx)
        {
            if (ctx.is_last_method_ret)
                ctx.unactive_nodes_stack.Push(this);
        }


        public override void do_back(BT_Context ctx)
        {
            var is_last = ctx.get_next_node() != this;

            if (is_last)
            {
                ref var ret = ref ctx.is_last_method_ret;
                ret = !ret;
            }
        }

    }
}


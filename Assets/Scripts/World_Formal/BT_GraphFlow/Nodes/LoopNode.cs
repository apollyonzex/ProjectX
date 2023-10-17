using GraphNode;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class LoopNode : BT_Node
    {
        [ShowInBody(format = "{0}")]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public BT_Expression count;

        public Enum.EN_LoopNode_Type loop_type;

        public List<(MethodInfo mi, BT_Node node)> loop_childs;

        public Stack<LoopNode> loop_nodes_stack;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> i { get; set; }
        #endregion


        #region Output
        [Output]
        [Display("out", seq = 1)]
        public void o(BT_Context ctx)
        {
            ctx.loop_nodes_stack.Push(this);
            loop_childs = new();
            loop_nodes_stack = new();
        }
        #endregion


        public override void do_back(BT_Context ctx)
        {
            switch (loop_type)
            {
                case Enum.EN_LoopNode_Type._for:
                    _for(ctx);
                    break;

                case Enum.EN_LoopNode_Type._until:
                    _until(ctx);
                    break;

                case Enum.EN_LoopNode_Type._while:
                    _while(ctx);
                    break;

                default:
                    _for(ctx);
                    break;
            }

            ctx.is_last_method_ret = true;
        }


        /// <summary>
        /// 单次处理
        /// </summary>
        public void single_opr(BT_Context ctx)
        {
            foreach (var (mi, node) in loop_childs)
            {
                if (node is LoopNode c)
                {
                    if (mi.Name == "o")
                        this.loop_nodes_stack.Push(c);

                    if (mi.Name == "do_back")
                        this.loop_nodes_stack.Pop();
                }

                ctx.@do(1, node, mi);
                if (loop_nodes_stack.Any())
                {
                    foreach (var loop_node in loop_nodes_stack)
                    {
                        loop_node.loop_childs.Add((mi, node));
                    }
                }
            }
        }


        public void _for(BT_Context ctx)
        {
            ctx.loop_nodes_stack.Pop();

            var _count = count.do_calc_int(ctx);
            _count--;

            if (_count <= 0) return;

            loop_childs.RemoveAt(0);

            for (int i = 0; i < _count; i++)
            {
                single_opr(ctx);
            }

            loop_childs.Clear();
        }


        public void _until(BT_Context ctx)
        {
            ctx.loop_nodes_stack.Pop();

            loop_childs.RemoveAt(0);

            while (!ctx.is_last_method_ret)
            {
                single_opr(ctx);
            }

            loop_childs.Clear();
        }


        public void _while(BT_Context ctx)
        {
            ctx.loop_nodes_stack.Pop();

            loop_childs.RemoveAt(0);

            while (ctx.is_last_method_ret)
            {
                single_opr(ctx);
            }

            loop_childs.Clear();
        }
    }
}


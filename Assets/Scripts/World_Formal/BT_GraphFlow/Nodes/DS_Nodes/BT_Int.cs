using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Int : BT_CPN
    {
        [ExprConst("value")]
        public int value => node.value.do_calc_int(ctx);

        BT_IntNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_IntNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
        }


    }
}


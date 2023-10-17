using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Bool : BT_CPN
    {
        [ExprConst("value")]
        public bool value => node.value.do_calc_bool(ctx);

        BT_BoolNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_BoolNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
        }


    }
}


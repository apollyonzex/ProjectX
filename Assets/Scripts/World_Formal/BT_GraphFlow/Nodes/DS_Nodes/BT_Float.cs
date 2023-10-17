using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Float : BT_CPN
    {
        [ExprConst("value")]
        public float value => node.value.do_calc_float(ctx);

        BT_FloatNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_FloatNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
        }


    }
}


using Battle.Enemys.BT_GraphFlow.Nodes;
using CalcExpr;
using System;

namespace Battle.Enemys.BT_GraphFlow.CPNs
{
    public class BT_Bool : BT_CPN
    {
        [ExprConst("value")]
        public bool value => node.value.do_calc_bool(ctx);

        //bool m_value = false;

        BT_BoolNode node;

        //================================================================================================

        public override void init(BT_Context ctx, BT_DSNode dn)
        {
            if (dn is BT_BoolNode node)
                this.node = node;

            this.ctx = ctx;
        }
    }
}


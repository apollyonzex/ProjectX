using Battle.Enemys.BT_GraphFlow.Nodes;
using CalcExpr;
using System;

namespace Battle.Enemys.BT_GraphFlow.CPNs
{
    public class BT_Float : BT_CPN
    {
        [ExprConst("value")]
        public float value => m_value;

        float m_value;

        BT_FloatNode node;

        //================================================================================================

        public override void init(BT_Context ctx, BT_DSNode dn)
        {
            if (dn is BT_FloatNode node)
                this.node = node;

            this.ctx = ctx;
        }

    }
}


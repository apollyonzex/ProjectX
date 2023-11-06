using Battle.Enemys.BT_GraphFlow.Nodes;
using CalcExpr;

namespace Battle.Enemys.BT_GraphFlow.CPNs
{
    public class BT_Bool : BT_CPN
    {
        [ExprConst("value")]
        public bool value => m_value;
        bool m_value;

        BT_BoolNode node;

        //================================================================================================

        public override void init(BT_Context ctx, BT_DSNode dn)
        {
            if (dn is BT_BoolNode node)
                this.node = node;

            refresh_data(ctx);
        }


        public override void refresh_data(BT_Context ctx)
        {
            m_value = node.value.do_calc_bool(ctx);
        }
    }
}


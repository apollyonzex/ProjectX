using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_ShareBool : BT_CPN
    {
        [ExprConst("value")]
        public bool value => m_value;

        public bool m_value;

        BT_Context ctx;
        BT_ShareBoolNode node;


        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_ShareBoolNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
            m_value = node.value.do_calc_bool(ctx);
        }


        public void reset(bool bl)
        {
            m_value = bl;
        }


    }
}


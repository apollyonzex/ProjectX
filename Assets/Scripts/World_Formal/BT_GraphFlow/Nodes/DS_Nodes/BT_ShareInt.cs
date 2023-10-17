using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_ShareInt : BT_CPN
    {
        [ExprConst("value")]
        public int value => m_value;

        public int m_value;

        BT_ShareIntNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_ShareIntNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
            m_value = node.value.do_calc_int(ctx);
        }


        public void reset(int f)
        {
            m_value = f;
        }


    }
}


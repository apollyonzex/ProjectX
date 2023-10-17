using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_ShareFloat : BT_CPN
    {
        [ExprConst("value")]
        public float value => m_value;

        public float m_value;

        BT_ShareFloatNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_ShareFloatNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
            m_value = node.value.do_calc_float(ctx);
        }


        public void reset(float f)
        {
            m_value = f;
        }


    }
}


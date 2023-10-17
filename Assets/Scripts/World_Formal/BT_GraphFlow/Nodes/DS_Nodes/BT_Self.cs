using CalcExpr;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Self : BT_CPN
    {
        public BT_SelfNode node;
        BT_Context ctx;

        [ExprConst("pos.x")]
        public float pos_x => ctx.position.x;

        [ExprConst("pos.y")]
        public float pos_y => ctx.position.y;

        [ExprConst("v.x")]
        public float velocity_x => ctx.v_self.x;

        [ExprConst("v.y")]
        public float velocity_y => ctx.v_self.y;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_SelfNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
        }
    }
}


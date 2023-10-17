using CalcExpr;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Caravan : BT_CPN
    {
        public BT_CaravanNode node;
        WorldContext wctx;

        [ExprConst("pos.x")]
        public float pos_x => wctx.caravan_pos.x;

        [ExprConst("pos.y")]
        public float pos_y => wctx.caravan_pos.y;

        [ExprConst("v.x")]
        public float velocity_x => wctx.caravan_velocity.x;

        [ExprConst("v.y")]
        public float velocity_y => wctx.caravan_velocity.y;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_CaravanNode node) return;
            this.node = node;

            wctx = WorldContext.instance;
        }
    }
}



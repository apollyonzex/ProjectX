using CalcExpr;
using System;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Vector2 : BT_CPN
    {
        [ExprConst("x")]
        public float x => node.x.do_calc_float(ctx);

        [ExprConst("y")]
        public float y => node.y.do_calc_float(ctx);

        [ExprConst("opr_x")]
        public float opr_x => node.get_result(ctx).x;

        [ExprConst("opr_y")]
        public float opr_y => node.get_result(ctx).y;

        BT_Vector2Node node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_Vector2Node node) return;
            this.node = node;      
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;
        }


    }
}


using CalcExpr;
using Common_Formal;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    public class BT_Random_Float : BT_CPN
    {
        [ExprConst("min")]
        public float min => node.min.do_calc_float(ctx);

        [ExprConst("max")]
        public float max => node.max.do_calc_float(ctx);

        [ExprConst("value")]
        public float value => m_value;

        float m_value;

        BT_Random_FloatNode node;
        BT_Context ctx;

        //================================================================================================

        public override void init(BT_DSNode e)
        {
            if (e is not BT_Random_FloatNode node) return;
            this.node = node;
        }


        public override void init(BT_Context ctx)
        {
            this.ctx = ctx;

            if (min > max)
                m_value = min;
            else
                do_rnd(); //规则：初始赋一个随机值
        }


        public void do_rnd()
        {
            m_value = EX_Utility.rnd_float(min, max);
        }


    }
}


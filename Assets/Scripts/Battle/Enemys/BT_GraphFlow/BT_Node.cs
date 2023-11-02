﻿using GraphNode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Enemys.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Node : Node
    {

    }


    /// <summary>
    /// 决策节点
    /// </summary>
    [System.Serializable]
    public class BT_Decide_Node : BT_Node
    {
        protected int seq;

        //==================================================================================================

        public virtual void do_back(BT_Context ctx)
        { 
        }


        protected void jump_out(BT_Context ctx)
        {
            seq = 0;
            ctx.decide_nodes.Pop();
            ctx.try_do_back();
        }


        protected System.Action<BT_Context> select_ac(int _seq)
        {
            var mi = GetType().GetMethod($"get__o{_seq}");
            return (System.Action<BT_Context>)mi?.Invoke(this, null);
        }
    }
}


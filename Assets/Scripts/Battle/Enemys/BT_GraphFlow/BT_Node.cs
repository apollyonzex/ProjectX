using GraphNode;
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
        public virtual void do_back(BT_Context ctx)
        { 
        }
    }
}


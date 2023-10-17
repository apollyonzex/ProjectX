using GraphNode;
using System;
using UnityEngine;
using World_Formal.Enemys.Projectiles;

namespace World_Formal.BT_GraphFlow.Nodes.Func_Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Take_DamageNode : BT_FuncNode
    {

        //================================================================================================

        #region Input
        [Display("do")]
        [Input]
        public void _i(BT_Context ctx, Projectile cell)
        {
            cell.hurt_target();
        }
        #endregion
    }
}


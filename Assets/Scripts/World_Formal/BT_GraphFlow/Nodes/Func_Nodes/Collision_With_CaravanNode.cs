using GraphNode;
using System;
using UnityEngine;
using World_Formal.Enemys.Projectiles;

namespace World_Formal.BT_GraphFlow.Nodes.Func_Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Collision_With_CaravanNode : BT_FuncNode
    {

        //================================================================================================

        #region Input
        [Display("in")]
        [Input]
        public void _i(BT_Context ctx, Projectile cell)
        {
            if (cell.target is not Caravans.Caravan) return;
            _o?.Invoke(ctx, cell);
        }
        #endregion


        #region Output
        [Display("events")]
        [Output(can_multi_connect = true)]
        public Action<BT_Context, Projectile> _o { get; set; }
        #endregion
    }
}


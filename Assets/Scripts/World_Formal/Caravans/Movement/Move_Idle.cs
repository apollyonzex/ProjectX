using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.Caravans.Movement
{
    public class Move_Idle : Caravan_Move_Helper.IMove_AC
    {
        void Caravan_Move_Helper.IMove_AC.@do(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            ctx.caravan_acc = Vector2.zero;
            ctx.caravan_velocity = Vector2.zero;
        }
    }
}


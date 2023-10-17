using System;
using World_Formal.Helpers;
using UnityEngine;

namespace World_Formal.Caravans.Movement
{
    public class Move_Land : Caravan_Move_Helper.IMove_AC
    {
        void Caravan_Move_Helper.IMove_AC.@do(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            ref var velocity = ref ctx.caravan_velocity;

            if (Road_Info_Helper.try_get_slope(ctx.caravan_pos.x, out var slope))
            {
                Vector2 vec = new(MathF.Cos(slope), MathF.Sin(slope));
                var vm_new = Vector2.Dot(velocity, vec);
                velocity = vm_new * vec;
            }

            ctx.caravan_liftoff_status = Common_Formal.Enum.EN_caravan_liftoff_status.ground;
            ctx.caravan_anim_trigger_status = Common_Formal.Enum.EN_caravan_anim_trigger_status.land;
        }
    }
}


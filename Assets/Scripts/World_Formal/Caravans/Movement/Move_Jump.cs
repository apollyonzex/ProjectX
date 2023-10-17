using System;
using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.Caravans.Movement
{
    public class Move_Jump : Caravan_Move_Helper.IMove_AC
    {
        void Caravan_Move_Helper.IMove_AC.@do(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            var gravity = Common.Config.current.gravity;
            ref var input_vy = ref ctx.input_jump_vy;
            if (ctx.is_jump_need_calc_velocity)
                input_vy = MathF.Sqrt(2 * ctx.input_jump_height * -gravity);

            ref var velocity = ref ctx.caravan_velocity;
            velocity.y = input_vy;

            ctx.caravan_liftoff_status = Common_Formal.Enum.EN_caravan_liftoff_status.sky;
            ctx.jump_peak = Mathf.Ceil(input_vy / -gravity * Common.Config.PHYSICS_TICKS_PER_SECOND);
            ctx.jump_floating = mgr.cell._desc.f_jump_floating_tick;

            if (ctx.is_enter_jumping_this_delta)
                enter_jumping(ctx, mgr);
            else
                ctx.is_enter_jumping_this_delta = true;
        }


        void enter_jumping(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            ctx.is_enter_jumping_this_delta = false;
            ctx.caravan_move_status = Common_Formal.Enum.EN_caravan_move_status.jumping;
            ctx.caravan_anim_trigger_status = Common_Formal.Enum.EN_caravan_anim_trigger_status.jump;

            Caravan_Move_Helper.instance.move(ctx, mgr);
        }
    }
}


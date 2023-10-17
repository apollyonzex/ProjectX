using Common;
using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.Caravans.Movement
{
    public class Move_Jumping : Caravan_Move_Helper.IMove_AC
    {
        void Caravan_Move_Helper.IMove_AC.@do(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            var acc = Vector2.zero;
            var gravity = Config.current.gravity;

            ref var t_peak = ref ctx.jump_peak;
            ref var t_floating = ref ctx.jump_floating;

            if (t_peak > 0) //上升
            {
                t_peak--;
                acc.y = gravity;
            }
            else if (t_peak == 0) //已达峰，进入浮空
            {
                if (t_floating > 0) //浮空
                {
                    t_floating--;
                    acc.y = 0;
                }
                else if (t_floating <= 0) //浮空结束
                {
                    t_peak--;
                    acc.y = gravity;
                }
            }
            else if (t_peak < 0) //下落
            {
                acc.y = gravity;            
            }

            ref var velocity = ref ctx.caravan_velocity;
            ref var pos = ref ctx.caravan_pos;

            velocity += acc * Config.PHYSICS_TICK_DELTA_TIME;
            pos += velocity * Config.PHYSICS_TICK_DELTA_TIME;


            //着陆判定
            if (Road_Info_Helper.try_get_altitude(pos.x, out var altitude))
            {
                if (pos.y <= altitude) //已着陆
                    enter_land(ctx, mgr);
            }
        }


        void enter_land(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            ctx.caravan_move_status = Common_Formal.Enum.EN_caravan_move_status.land;
            Caravan_Move_Helper.instance.move(ctx, mgr);
        }
    }
}


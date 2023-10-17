using Common;
using Common_Formal;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.Caravans;
using World_Formal.Caravans.Movement;

namespace World_Formal.Helpers
{
    public class Caravan_Move_Helper : Common_Formal.Singleton<Caravan_Move_Helper>
    {
        public interface IMove_AC
        {
            void @do(WorldContext ctx, CaravanMgr_Formal mgr);
        }

        Dictionary<Enum.EN_caravan_move_status, IMove_AC> m_ac_dic = new()
        {
            { Enum.EN_caravan_move_status.idle, new Move_Idle() },
            { Enum.EN_caravan_move_status.run, new Move_Run() },
            { Enum.EN_caravan_move_status.jump, new Move_Jump() },
            { Enum.EN_caravan_move_status.jumping, new Move_Jumping() },
            { Enum.EN_caravan_move_status.land, new Move_Land() },
        };


        Dictionary<uint, System.Func<Dictionary<Enum.EN_caravan_move_status, IMove_AC>>> m_select_dic = new()
        {
            { 300000001, new Caravans.Movement.FireBoat.Move_Init().get_dic },
        };

        //==================================================================================================

        /// <summary>
        /// 初始化配置
        /// </summary>
        public void init(CaravanMgr_Formal mgr)
        {
            if (m_select_dic.TryGetValue(mgr.cell_id, out var func))
            {
                m_ac_dic = func.Invoke();
            }
        }


        //==================================================================================================

        public void move(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            var state = ctx.caravan_move_status;
            m_ac_dic[state].@do(ctx, mgr);

            if (ctx.caravan_liftoff_status == Enum.EN_caravan_liftoff_status.ground)
            {
                valid_idle_or_run(ctx);
            }
        }


        public void valid_idle_or_run(WorldContext ctx)
        {
            ref var state = ref ctx.caravan_move_status;
            if (ctx.caravan_velocity.x == 0 && ctx.caravan_acc_status == Enum.EN_caravan_acc_status.braking) 
                state = Enum.EN_caravan_move_status.idle;
            else
                state = Enum.EN_caravan_move_status.run;
        }


        public void jump(WorldContext ctx, float height, bool is_jumping_this_delta)
        {
            ctx.input_jump_height = height;
            ctx.is_jump_need_calc_velocity = true;

            do_jump(ctx, is_jumping_this_delta);
        }


        public void jump(WorldContext ctx, Vector2 velocity, bool is_jumping_this_delta)
        {
            ctx.input_jump_vy = velocity.y;
            ctx.is_jump_need_calc_velocity = false;

            do_jump(ctx, is_jumping_this_delta);
        }


        void do_jump(WorldContext ctx, bool is_jumping_this_delta)
        {
            ctx.is_enter_jumping_this_delta = is_jumping_this_delta;
            ctx.caravan_move_status = Enum.EN_caravan_move_status.jump;

            Mission.instance.try_get_mgr(Config.CaravanMgr_Name, out CaravanMgr_Formal mgr);
            move(ctx, mgr);
        }


        public void move(WorldContext ctx)
        {
            ctx.caravan_acc_status = Enum.EN_caravan_acc_status.driving;
            ctx.caravan_move_status = Enum.EN_caravan_move_status.run;
        }


        public void stop(WorldContext ctx)
        {
            ctx.caravan_acc_status = Enum.EN_caravan_acc_status.braking;
        }


        /// <summary>
        /// 计算并设置篷车倾斜弧度
        /// </summary>
        public void calc_and_set_caravan_leap_rad(WorldContext ctx)
        {
            if (ctx.caravan_liftoff_status == Enum.EN_caravan_liftoff_status.ground)
            {
                Road_Info_Helper.try_get_leap_rad(ctx.caravan_pos.x, out var rad);
                var rot_angle_limit = Config.current.caravan_rotate_angle_limit * Mathf.Deg2Rad;
                float _target_rad = Mathf.Clamp(rad, -rot_angle_limit, rot_angle_limit);
                ctx.caravan_rad += (_target_rad - ctx.caravan_rad) * Config.current.caravan_rotate_speed * Config.PHYSICS_TICK_DELTA_TIME;
            }
        }
    }
}


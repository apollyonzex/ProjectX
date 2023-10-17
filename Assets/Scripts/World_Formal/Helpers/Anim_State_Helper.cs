using Common;
using Common_Formal;
using Common_Formal.DS;
using Common_Formal.Helpers;
using System.Collections.Generic;

namespace World_Formal.Helpers
{
    public class Anim_State_Helper : Common_Formal.Singleton<Anim_State_Helper>
    {
        Dictionary<Enum.EN_caravan_move_status, System.Action<WorldContext>> m_lasting_action_dic = new()
        {
            { Enum.EN_caravan_move_status.idle, idle },
            { Enum.EN_caravan_move_status.run, run },
            { Enum.EN_caravan_move_status.jumping, jumping },
        };

        Dictionary<Enum.EN_caravan_anim_trigger_status, System.Action<WorldContext>> m_trigger_action_dic = new()
        {
            { Enum.EN_caravan_anim_trigger_status.land, land },
            { Enum.EN_caravan_anim_trigger_status.jump, jump },
        };

        //==================================================================================================

        public SpineDS choose_and_load(SpineDS ds, WorldContext ctx)
        {
            var name = ctx.caravan_anim_status.ToString();
            name = (string)typeof(SpineDS).GetField(name).GetValue(ds);
            Spine_Load_Helper.instance.@do(ds, name);

            return ds;
        }


        public void set_state(WorldContext ctx)
        {
            var move_status = ctx.caravan_move_status;
            m_lasting_action_dic[move_status]?.Invoke(ctx);

            var trigger_status = ctx.caravan_anim_trigger_status;
            if (trigger_status != Enum.EN_caravan_anim_trigger_status.none)
            {
                m_trigger_action_dic[trigger_status]?.Invoke(ctx);
            }
        }


        static void idle(WorldContext ctx)
        {
            ctx.caravan_anim_status = Enum.EN_caravan_anim_status.idle;
        }


        static void run(WorldContext ctx)
        {
            if (ctx.caravan_velocity.x <= Config.current.anim_idle_when_below_velocity) //规则：当速度小于一定值，播idle动画
            {
                ctx.caravan_anim_status = Enum.EN_caravan_anim_status.idle;
                return;
            }

            var acc_st = ctx.caravan_acc_status;
            if (acc_st == Enum.EN_caravan_acc_status.driving)
            {
                ctx.caravan_anim_status = Enum.EN_caravan_anim_status.run;
                return;
            }

            if (acc_st == Enum.EN_caravan_acc_status.braking)
            {
                ctx.caravan_anim_status = Enum.EN_caravan_anim_status.brake;
                return;
            }    
        }


        static void jumping(WorldContext ctx)
        {
            ctx.caravan_anim_status = Enum.EN_caravan_anim_status.jumping;
        }


        static void jump(WorldContext ctx)
        {
            ctx.caravan_anim_status = Enum.EN_caravan_anim_status.jump;
        }


        static void land(WorldContext ctx)
        {
            ctx.caravan_anim_status = Enum.EN_caravan_anim_status.land;
        }
    }
}


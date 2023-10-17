using Common_Formal;
using World_Formal.BT_GraphFlow;

namespace World_Formal.Helpers
{
    public class Enemy_State_Helper : Singleton<Enemy_State_Helper>
    {

        //================================================================================================

        public void calc_state(BT_Context ctx)
        {
            ref var main_state = ref ctx.main_State;

            if (!ctx.is_alive) //死亡
            {
                if (ctx.is_in_sky) //规则：坠落
                {
                    main_state = Enemys.Enum.EN_Main_State.Drop;
                    return;
                }
                else //规则：倒地
                {
                    main_state = Enemys.Enum.EN_Main_State.FallDown;
                    return;
                }
            }

            if (ctx.is_jump) //规则：跳跃
            {
                main_state = Enemys.Enum.EN_Main_State.Jump;
                return;
            }

            if (ctx.v_self.magnitude > 0.01f) //规则：移动
            {
                main_state = Enemys.Enum.EN_Main_State.Move;
                return;
            }

            main_state = Enemys.Enum.EN_Main_State.Idle; //默认：待机
        }
    }
}


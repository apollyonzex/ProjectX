using System;
using UnityEngine;
using World_Formal.BT_GraphFlow;
using World_Formal.Enemys.Projectiles;

namespace World_Formal.Enemys.Move_AC
{
    public class E_Ground_Move : IEnemy_Move
    {
        void IEnemy_Move.init_move_prms(BT_Context ctx)
        {
            ctx.is_use_gravity = true;
        }


        void IEnemy_Move.jump(BT_Context ctx, float vx, float height)
        {
            if (ctx.is_jump) return; //规则：跳跃不可嵌套

            ctx.v_impact.x = vx;

            var t = 2 * -Common.Config.current.enemy_gravity * height;
            if (t > 0)
            {
                ctx.v_enviroment.y = MathF.Sqrt(t);
                ctx.is_jump = true;
            }
        }


        bool IEnemy_Move.try_create_projectile(BT_Context ctx, uint id, Vector2 pos, Vector2 dir, float velocity, out Projectile projectile)
        {
            projectile = null;
            return false;
        }


        void IEnemy_Move.move(BT_Context ctx, Vector2 velocity)
        {
            //规则：跳跃时自身速度置为0
            if (ctx.is_jump)
            {
                ctx.v_self = Vector2.zero;
                return;
            }

            ctx.v_self = velocity;
        }


        void IEnemy_Move.notify_on_land(BT_Context ctx)
        {
            ref var is_jump = ref ctx.is_jump;
            if (is_jump)
                is_jump = false;
        }
    }
}


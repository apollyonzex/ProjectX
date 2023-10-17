using Common;
using Common_Formal;
using UnityEngine;
using World_Formal.BT_GraphFlow;
using World_Formal.Enemys.Projectiles;

namespace World_Formal.Enemys.Move_AC
{
    public class E_Fly_Move : IEnemy_Move
    {
        void IEnemy_Move.init_move_prms(BT_Context ctx)
        {
            ctx.is_use_gravity = false;
        }


        void IEnemy_Move.jump(BT_Context ctx, float vx, float height)
        {
        }


        bool IEnemy_Move.try_create_projectile(BT_Context ctx, uint id, Vector2 pos, Vector2 dir, float velocity, out Projectile projectile)
        {
            Mission.instance.try_get_mgr(Config.ProjectileMgr_Enemy_Name, out ProjectileMgr pmgr);
            return pmgr.try_add_cell(id, pos, dir, velocity, ctx.target, out projectile);
        }


        void IEnemy_Move.move(BT_Context ctx, Vector2 velocity)
        {
            ctx.v_self = velocity;
        }


        void IEnemy_Move.notify_on_land(BT_Context ctx)
        {
        }
    }
}


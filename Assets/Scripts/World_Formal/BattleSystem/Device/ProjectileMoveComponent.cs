
using System.Collections.Generic;
using World_Formal.BattleSystem.DeviceGraph;

namespace World_Formal.BattleSystem.Device
{
    public class ProjectileMoveComponent : ProjectileComponent
    {
        public override void tick(DeviceContext ctx, Projectile p)
        {
            move(ctx,p);
        }

        public void move(DeviceContext ctx,Projectile p)
        {
            if (p.freeze == true)
            {
                p.freeze = false;
                return;
            }

            var v = p.velocity + new UnityEngine.Vector2(0,p.desc.f_gravity * Common.Config.PHYSICS_TICK_DELTA_TIME);

            p.direction = v.normalized;     //受重力改变朝向
            p.velocity = v;                 //改变速度
            p.position += new UnityEngine.Vector2(p.velocity.x,p.velocity.y) * Common.Config.PHYSICS_TICK_DELTA_TIME;
        }
    }
}

using Common_Formal;
using System.Collections.Generic;
using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.DeviceGraph.Nodes;
using World_Formal.DS;

namespace World_Formal.BattleSystem.Device
{
    public class ProjectileDefaultComponent : ProjectileComponent
    {
        public override DeviceNode graph_node => m_node;

        private EmitterNode m_node;

        private int lifetime;
        private int max_hit;

        private List<(ITarget target, bool stay)> hit_colliders = new List<(ITarget, bool)>();

        public ProjectileDefaultComponent(EmitterNode node,Projectile p)
        {
            m_node = node;
            lifetime = p.desc.f_countdown;
            max_hit = p.desc.f_max_hit;
        }

        //记录飞射物基本的逻辑:移动,生存时长,击中效果等...
        public override void tick(DeviceContext ctx, Projectile p)
        {
            live(ctx, p);
            hit(ctx, p);
            move(ctx, p);
            UnityEngine.Debug.Log("飞射物tick");
        }

        private void move(DeviceContext ctx, Projectile p)
        {
            if (p.freeze == true)
            {
                p.freeze = false;
                return;
            }

            var v = p.velocity + new UnityEngine.Vector2(0, p.desc.f_gravity * Common.Config.PHYSICS_TICK_DELTA_TIME);

            p.direction = v.normalized;     //受重力改变朝向
            p.velocity = v;                 //改变速度
            p.position += new UnityEngine.Vector2(p.velocity.x, p.velocity.y) * Common.Config.PHYSICS_TICK_DELTA_TIME;
        }
        private void live(DeviceContext ctx,Projectile p)
        {
            lifetime--;
            if(lifetime <= 0)
            {   
                destroy(ctx,p);
            }
        }
        private void hit(DeviceContext ctx,Projectile p)
        {
            var targets = BattleUtility.select_all_target_in_circle(p.position, p.desc.f_detection_range, p.faction);

            for(int i = 0; i < hit_colliders.Count; i++)
            {
                hit_colliders[i] = (hit_colliders[i].target, false);
            }

            foreach(var target in targets)
            {
                if (!insert_target(target))
                {
                    colliding(ctx,p,target);
                }
            }

            for (int i = 0; i < hit_colliders.Count;)
            {
                if (!hit_colliders[i].stay)
                {
                    var last = hit_colliders.Count - 1;
                    if (i != last)
                    {
                        hit_colliders[i] = hit_colliders[last];
                    }
                    hit_colliders.RemoveAt(last);
                }
                else
                {
                    ++i;
                }
            }
        }
        private bool insert_target(ITarget target)
        {
            for (int i = 0, c = hit_colliders.Count; i < c; ++i)
            {
                if (hit_colliders[i].target == target)
                {
                    hit_colliders[i] = (target, true);
                    return true;
                }
            }
            hit_colliders.Add((target, true));
            return false;
        }
        private  void destroy(DeviceContext ctx,Projectile p)
        {
            p.destroy();
        }
        private void colliding(DeviceContext ctx,Projectile p,ITarget target)
        {
            if(target is Enemys.Projectiles.ProjectileView)
            {
                m_node?.on_hit_projectile?.invoke(ctx, new ProjectileEvent
                {
                    p = p,
                    target = target,
                    normal = new DeviceVector2(target.Position - p.position, false),
                });
            }
            else
            {
                m_node?.on_hit_target?.invoke(ctx, new ProjectileEvent
                {
                    p = p,
                    target = target,
                    normal = new DeviceVector2(target.Position - p.position, false),
                });
            }

            max_hit--;
            if (max_hit <= 0)
            {
                destroy(ctx,p);
            }
        }
    }
}

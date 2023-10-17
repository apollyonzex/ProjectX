using Common_Formal;
using System;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.DS;
using World_Formal.Helpers;

namespace World_Formal.Enemys.Projectiles
{
    public class Projectile : ITarget
    {
        public AutoCode.Tables.Projectile.Record _desc;

        public float hp;
        public int hit;
        public int life;
        public bool is_alive => valid_alive();

        public Vector2 position;
        public Vector2 direction;
        public float velocity;
        public Collider2D collider;
        
        public Vector2 view_position => position;
        public Quaternion view_rotation => EX_Utility.quick_look_rotation_from_left(direction);

        Vector2 ITarget.Position => position;
        Collider2D ITarget.collider => collider;

        public ITarget target;

        public ProjectileMgr mgr;

        public Action do_when_collide_target;
        public Action do_when_collide_ground;
        public Action do_when_collide_projectile;

        //==================================================================================================

        public Projectile(ProjectileMgr mgr, uint id, Vector2 pos, Vector2 dir, float velocity, ITarget target)
        {
            this.mgr = mgr;
            position = pos;
            direction = dir;
            this.velocity = velocity;
            this.target = target;

            DB.instance.projectile.try_get(id, out _desc);

            hp = _desc.f_projectile_hp;
            life = _desc.f_countdown;
            hit = _desc.f_max_hit;
        }


        public void tick()
        {
            if (!is_alive) //检定：是否存活
            {
                mgr.set_del(this);
                return;
            }

            position += Common.Config.PHYSICS_TICK_DELTA_TIME * velocity * direction;

            if (valid_collide_target()) //检定：是否撞到目标
            {
                do_when_collide_target?.Invoke();
                hit--;
            }

            if (valid_collide_ground()) //检定：是否撞到地面
            {
                do_when_collide_ground?.Invoke();
                hit--;
            }

            life--;
        }


        /// <summary>
        /// 检定：子弹是否存活
        /// </summary>
        public bool valid_alive()
        {
            if (hp <= 0) return false;
            if (hit < 0) return false;
            if (life <= 0) return false;

            return true;
        }


        public bool valid_collide_target()
        {
            var bl = target.collider.bounds.Intersects(collider.bounds);
            return bl;
        }


        public bool valid_collide_ground()
        {
            Road_Info_Helper.try_get_altitude(position.x, out var altitude);
            return position.y <= altitude;
        }


        public void hurt_target()
        {
            target.hurt(_desc.f_projectile_dmg);
            Debug.Log($"篷车剩余hp：{WorldContext.instance.caravan_hp}");
        }


        void ITarget.hurt(int dmg)
        {
            
        }
    }
}


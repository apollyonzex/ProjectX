using System.Collections.Generic;
using UnityEngine;
using Common_Formal;
using AutoCode.Tables;
using World_Formal.Enemys;
using World_Formal.Enemys.Move_AC;
using World_Formal.BT_GraphFlow;
using System;

namespace World_Formal.Helpers
{
    public class Enemy_Move_Helper : Singleton<Enemy_Move_Helper>
    {
        Dictionary<MonsterNormal.e_moveType, IEnemy_Move> m_move_dic = new()
        {
            { MonsterNormal.e_moveType.i_fly , new E_Fly_Move() },
            { MonsterNormal.e_moveType.i_ground , new E_Ground_Move() },
        };

        //================================================================================================

        public void init_move_prms(BT_Context ctx)
        {
            if (m_move_dic.TryGetValue(ctx.cell.move_type, out var ac))
                ac.init_move_prms(ctx);
        }


        public void set_position(BT_Context ctx)
        {
            ref var pos = ref ctx.position;
            ref var altitude = ref ctx.altitude;

            bool is_need_valid_land = false; //是否需要检测落地
            if (ctx.is_use_gravity) //重力检定
            {
                if (pos.y >= altitude)
                {
                    ctx.v_enviroment.y += Common.Config.current.enemy_gravity * Common.Config.PHYSICS_TICK_DELTA_TIME;
                    is_need_valid_land = true;
                }
                else
                {
                    ctx.v_enviroment.y = 0;
                }
            }
            else
            {
                ctx.v_enviroment.y = 0;
            }

            pos += ctx.velocity * Common.Config.PHYSICS_TICK_DELTA_TIME;
            altitude = Road_Info_Helper.try_get_altitude(pos.x);

            if (pos.y <= altitude) //规则：怪物位置不可低于地面
            {
                pos.y = altitude;

                if (is_need_valid_land) //触发落地
                    land(ctx);
            }

            if (!ctx.is_in_sky && ctx.v_impact.x != 0) //规则：未“滞空”的敌人，这一速度会随时间逐渐降低，最终使这一速度降低为0
                decrease_impact(ctx);
        }


        public void move(BT_Context ctx, Vector2 velocity)
        {
            if (m_move_dic.TryGetValue(ctx.cell.move_type, out var ac))
                ac.move(ctx, velocity);
        }


        public void jump(BT_Context ctx, float vx, float height)
        {
            if (m_move_dic.TryGetValue(ctx.cell.move_type, out var ac))
                ac.jump(ctx, vx, height);
        }


        public void land(BT_Context ctx)
        {
            if (m_move_dic.TryGetValue(ctx.cell.move_type, out var ac))
                ac.notify_on_land(ctx);
        }


        public void follow_caravan(BT_Context bctx, Vector2 offset)
        {
            var ctx = WorldContext.instance;

            bctx.v_self = Vector2.zero;
            bctx.is_use_gravity = false;

            EX_Utility.calc_pos_from_rotate(ref offset, ctx.caravan_rad);
            bctx.position = offset + ctx.caravan_pos;
            bctx.direction = ctx.caravan_dir;

            if (ctx.is_need_reset) //特殊处理：防止被多减一次
            {
                bctx.position.x += ctx.reset_dis;
            }
        }


        public void follow_caravan_when_hold(BT_Context bctx, Vector2 offset)
        {
            var ctx = WorldContext.instance;

            bctx.v_self = Vector2.zero;
            bctx.is_use_gravity = false;

            EX_Utility.calc_pos_from_rotate(ref offset, ctx.caravan_rad);
            bctx.position = offset + ctx.caravan_pos;

            bctx.direction = ctx.caravan_dir;

            if (ctx.is_need_reset) //特殊处理：防止被多减一次
            {
                bctx.position.x += ctx.reset_dis;
            }
        }


        /// <summary>
        /// 逐渐衰减impact
        /// </summary>
        void decrease_impact(BT_Context ctx)
        {
            ref var impact = ref ctx.v_impact;
            impact = MathF.Max(0, impact.magnitude - Common.Config.current.impact_a * Common.Config.PHYSICS_TICK_DELTA_TIME) * impact.normalized;
        }


        /// <summary>
        /// 创建子弹
        /// </summary>
        public bool try_create_projectile(BT_Context ctx, uint id, Vector2 dir, float velocity, out Enemys.Projectiles.Projectile projectile)
        {
            projectile = null;

            var pos = ctx.position; //临时
            dir = dir.normalized;

            if (m_move_dic.TryGetValue(ctx.cell.move_type, out var ac))
                return ac.try_create_projectile(ctx, id, pos, dir, velocity, out projectile);

            return false;
        }
    }
}


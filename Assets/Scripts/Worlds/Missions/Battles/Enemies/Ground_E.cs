using Common;
using UnityEngine;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;
using static Worlds.Missions.Battles.Enemies.BehaviourTrees.EnemyBrain_Enum;

namespace Worlds.Missions.Battles.Enemies
{
    public class Ground_E : IEnemy
    {
        readonly Enemy cell;
        readonly EnemyBrain brain;

        public event System.Action land_event; // 着陆事件
        public bool is_liftoff = false; // 是否滞空

        IEnemy ienemy;

        Vector2 IEnemy.environment_v => m_environment_v;
        Vector2 m_environment_v => gravity;
        Vector2 gravity;

        Vector2 IEnemy.impact_v => m_impact_v;

        Vector2 IEnemy.active_v => brain.velocity_result;

        bool IEnemy.no_gravity => brain.is_boarding && !cell.is_dead;

        bool IEnemy.no_brain => is_liftoff;

        Vector2 m_impact_v = Vector2.zero;
        Vector2 m_impact_dir;

        //================================================================================================


        public Ground_E(Enemy cell)
        {
            this.cell = cell;
            this.brain = cell.brain;
            this.ienemy = this;
        }


        public Ground_E(Enemy cell, Vector2 impact_v, Vector2 impact_dir)
        {
            this.cell = cell;
            this.brain = cell.brain;
            this.ienemy = this;

            m_impact_v = impact_v;
            m_impact_dir = impact_dir;
        }


        void IEnemy.calc_active()
        {
            brain.on_physics_tick();
        }


        void IEnemy.calc_impacting()
        {
            if (!cell.is_dead && (brain.is_boarding || brain.is_holding)) // 规则：正在顶车/扒车的怪物，击退无效
            {
                m_impact_v = Vector2.zero;
                return;
            }                

            if (m_impact_v == Vector2.zero) return; // 击退结束
            if (is_liftoff) return; // 规则：如果陆行怪物滞空，不消减击退力

            if (m_impact_dir.x > 0 || (m_impact_dir.x == 0 && m_impact_v.x >= 0))
            {
                m_impact_v.x += cell.impact_value_delta_decay;
                m_impact_v.x = m_impact_v.x <= 0 ? 0 : m_impact_v.x;
            }

            if (m_impact_dir.x < 0 || (m_impact_dir.x == 0 && m_impact_v.x < 0))
            {
                m_impact_v.x -= cell.impact_value_delta_decay;
                m_impact_v.x = m_impact_v.x >= 0 ? 0 : m_impact_v.x;
            }
        }


        void IEnemy.calc_environmet()
        {
            if (ienemy.no_gravity)
            {
                gravity.y = 0;
                return;
            }

            gravity.y += Config.current.enemy_gravity * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
        }


        void IEnemy.upd_position(ref Vector2 position)
        {
            position += cell.velocity * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;

            if (position.y <= 0) //着陆
            {
                position.y = 0;
                gravity.y = 0;
                m_impact_v.y = 0;
                is_liftoff = false;

                land_event?.Invoke(); //着陆事件

                if (cell.is_dead)
                    cell.set_anm_state((int)Main_State.Lie);
            }
        }


        void IEnemy.notify_on_impact(float dis, Vector2 dir)
        {
            var mass = cell.mass;
            var x = Mathf.Sqrt(2 * dis * Config.current.impact_a / mass) * dir.x;
            var y = Mathf.Sqrt(2 * dis * -Config.current.enemy_gravity / mass) * dir.y + -Config.current.enemy_gravity * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME; //规则: y轴只计算重力衰减

            m_impact_v = new Vector2(x, y);
            m_impact_dir = dir;
            is_liftoff = true;
        }


        void IEnemy.notify_on_dead(ref Vector2 active_v, ref Vector2 active_acc)
        {
            m_impact_v.x += active_v.x; // 规则：将角色水平方向的主动移速，添加至击退移速中
            gravity.y += active_v.y; // 规则：将角色竖直方向的主动移速，添加至环境移速中

            active_v = Vector2.zero; //规则：自身主动移速，加速度清零
            active_acc = Vector2.zero;

            cell.set_anm_state((int)Main_State.Dead);
        }

    }
}


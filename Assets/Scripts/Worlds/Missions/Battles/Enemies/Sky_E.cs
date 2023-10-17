using Common;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;
using static Worlds.Missions.Battles.Enemies.BehaviourTrees.EnemyBrain_Enum;

namespace Worlds.Missions.Battles.Enemies
{
    public class Sky_E : IEnemy
    {
        readonly Enemy cell;
        readonly EnemyBrain brain;

        IEnemy ienemy;

        Vector2 IEnemy.environment_v => Vector2.zero;

        Vector2 IEnemy.impact_v => m_impact_v;

        Vector2 IEnemy.active_v => brain.velocity_result;

        bool IEnemy.no_gravity => true;

        bool IEnemy.no_brain => false;

        Vector2 m_impact_v;
        Vector2 m_impact_dir;

        //================================================================================================


        public Sky_E(Enemy cell)
        {
            this.cell = cell;
            this.brain = cell.brain;
            this.ienemy = this;
        }

        void IEnemy.calc_active()
        {
            brain.on_physics_tick();
        }


        void IEnemy.calc_impacting()
        {
            if (m_impact_v == Vector2.zero) return;

            m_impact_v += cell.impact_value_delta_decay * m_impact_dir;

            if (m_impact_dir.x > 0 && m_impact_v.x <= 0) m_impact_v.x = 0;
            if (m_impact_dir.x < 0 && m_impact_v.x >= 0) m_impact_v.x = 0;

            if (m_impact_dir.y > 0 && m_impact_v.y <= 0) m_impact_v.y = 0;
            if (m_impact_dir.y < 0 && m_impact_v.y >= 0) m_impact_v.y = 0;
        }


        void IEnemy.calc_environmet()
        {
        }


        void IEnemy.upd_position(ref Vector2 position)
        {
            position += cell.velocity * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;

            if (position.y <= 0)
            {
                position.y = 0; //不可进入地下
            }        
        }


        void IEnemy.notify_on_impact(float dis, Vector2 dir)
        {
            var x = Mathf.Sqrt(2 * dis * Config.current.impact_a / cell.mass) * dir.x;
            var y = Mathf.Sqrt(2 * dis * Config.current.impact_a / cell.mass) * dir.y;

            m_impact_v = new Vector2(x, y);
            m_impact_dir = dir;
        }


        void IEnemy.notify_on_dead(ref Vector2 active_v, ref Vector2 active_acc)
        {
            var e = new Ground_E(cell, m_impact_v, m_impact_dir); // 规则：飞行怪一旦死亡，按陆行怪滞空结算
            e.is_liftoff = true;

            cell.concrete = e;
            cell.concrete.notify_on_dead(ref active_v, ref active_acc);

            cell.set_anm_state((int)Main_State.Dead);
        }
    }
}


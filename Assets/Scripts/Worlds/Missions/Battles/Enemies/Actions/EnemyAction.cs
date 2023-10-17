using AutoCode.Tables;
using Common;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;


namespace Worlds.Missions.Battles.Enemies.Actions
{
    public interface IEnemyAction
    {
        void init(EnemyBrain brain);
        void moving_to_target();
        void missile_attack();
        void melee_attack(int damage);
    }


    public class EnemyAction
    {
        IEnemyAction action;
        EnemyBrain brain;

        //================================================================================================


        public EnemyAction(EnemyBrain brain)
        {
            this.brain = brain;

            switch (brain.cell.move_type)
            {
                case EnemyNormal.e_moveType.i_ground:
                    action = new Ground_E_AC();
                    break;

                case EnemyNormal.e_moveType.i_fly:
                    action = new Sky_E_AC();
                    break;
            }

            action.init(brain);
        }


        public void moving_to_target()
        {
            action.moving_to_target();
        }


        public void stop_moving()
        {
            brain.velocity = Vector2.zero;
            brain.acc = Vector2.zero;
        }


        /// <summary>
        /// 远程攻击
        /// </summary>
        public void missile_attack()
        {
            action.missile_attack();
            brain.main_state = EnemyBrain_Enum.Main_State.Idle;
        }


        /// <summary>
        /// 近战攻击
        /// </summary>
        public void melee_attack()
        {
            if (brain.is_colliding_with_target)
            {
                action.melee_attack(brain.cell.melee_atk);
                brain.battle_state = EnemyBrain_Enum.Battle_State.Default;
            }        
        }


        /// <summary>
        /// 顶住大篷车
        /// </summary>
        public void hold()
        {
            if (brain.is_holding) return;
            if (brain.target is not BattleCaravan caravan) return;

            if (brain.is_colliding_with_target && caravan.liftoff_status == Liftoffstatus.ground)
            {
                var cell = brain.cell;
                cell.mgr.add_holding(cell);
            }
        }


        /// <summary>
        /// 跳跃
        /// 限定地面单位
        /// </summary>
        public void jump(float height, float vx)
        {
            if (action is not Ground_E_AC e) return;
            if (brain.is_jumping) return; // 禁止二段跳

            e.jump(height, vx);
        }


        /// <summary>
        /// 着陆
        /// 限定地面单位
        /// </summary>
        public void landing(ref int rest_ticks)
        {
            if (action is not Ground_E_AC e) return;

            e.landing(ref rest_ticks);
        }


        /// <summary>
        /// 扒车
        /// </summary>
        public void board()
        {
            if (brain.is_boarding) return;
            if (brain.target is not BattleCaravan) return;

            if (brain.is_colliding_with_target)
            {
                var cell = brain.cell;
                var mgr = cell.mgr;

                if (mgr.try_add_boarding(cell))
                    brain.main_state = EnemyBrain_Enum.Main_State.Boarding;               
            }
        }


        /// <summary>
        /// 正在扒车
        /// </summary>
        public void boarding(ref int rest_ticks)
        {
            if (rest_ticks > 0)
            {
                rest_ticks--;
                return;
            }

            var target = brain.target;
            var damage = Config.current.boarding_dot;
            target.current_hp -= damage;
            Debug.Log($"怪物 {brain.cell._id} 对 {target.name} 造成{damage}点扒车攻击!  剩余Hp: {target.current_hp}");

            rest_ticks = Config.current.boarding_atk_base / brain.cell.melee_atk;//进入cd
        }

    }


}


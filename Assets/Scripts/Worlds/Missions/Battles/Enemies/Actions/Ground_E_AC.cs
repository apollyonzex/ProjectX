using Common;
using UnityEngine;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;


namespace Worlds.Missions.Battles.Enemies.Actions
{
    public class Ground_E_AC : IEnemyAction
    {
        EnemyBrain brain;
        Enemy cell;
        Ground_E concrete;

        //================================================================================================


        void IEnemyAction.init(EnemyBrain brain)
        {
            this.brain = brain;
            this.cell = brain.cell;
            this.concrete = (Ground_E)cell.concrete;
        }


        void IEnemyAction.moving_to_target()
        {
            var vx = brain.velocity.x;
            var self_pos = cell.position;
            var target_pos = brain.target.position;
            Vector2 dir = (target_pos - self_pos).normalized;
            vx += vx * dir.x;
            brain.velocity.x = vx;
        }


        void IEnemyAction.missile_attack()
        {
            cell.device_tick();
        }


        void IEnemyAction.melee_attack(int damage)
        {
            var target = brain.target;
            target.current_hp -= damage;
            Debug.Log($"怪物 {brain.cell._id} 对 {target.name} 造成{damage}点近战攻击!  剩余Hp: {target.current_hp}");
        }


        /// <summary>
        /// 跳跃
        /// </summary>
        public void jump(float height, float vx)
        {
            brain.velocity.x = vx;
            brain.velocity.y = Mathf.Sqrt(2 * -Config.current.enemy_gravity * height);

            concrete.is_liftoff = true;
            concrete.land_event += land;
        }


        void land()
        {
            concrete.land_event -= land;

            if (brain.main_state == EnemyBrain_Enum.Main_State.Dead) return; 

            brain.velocity = Vector2.zero;
            brain.acc = Vector2.zero;

            brain.landing_rest_ticks = Config.current.enemy_landing_ticks;
            brain.main_state = EnemyBrain_Enum.Main_State.Landing;
        }


        /// <summary>
        /// 着陆中
        /// </summary>
        public void landing(ref int rest_ticks)
        {
            rest_ticks--;
            if (rest_ticks == 0) //规则: 等待指定帧数后，进入idle状态
                brain.main_state = EnemyBrain_Enum.Main_State.Idle;
        }

    }
}

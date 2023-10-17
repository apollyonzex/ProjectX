using UnityEngine;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;


namespace Worlds.Missions.Battles.Enemies.Actions
{
    public class Sky_E_AC : IEnemyAction
    {
        EnemyBrain brain;
        Enemy cell;

        //================================================================================================


        void IEnemyAction.init(EnemyBrain brain)
        {
            this.brain = brain;
            this.cell = this.brain.cell;
        }


        void IEnemyAction.moving_to_target()
        {
            var v = brain.velocity;
            var self_pos = cell.position;
            var target_pos = brain.target.position;
            Vector2 dir = (target_pos - self_pos).normalized;
            v += v.magnitude * dir;
            brain.velocity = v;
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


    }
}


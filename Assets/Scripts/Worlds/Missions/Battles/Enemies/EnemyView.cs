using Foundation;
using UnityEngine;


namespace Worlds.Missions.Battles.Enemies
{
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        public Animator anm;

        Enemy cell;
        EnemyMgr owner;

        //SpriteRenderer m_sprite;

        Enemy IEnemyView.cell => this.cell;

        float flip => cell.flip_x ? 180 : 0;

        //================================================================================================


        public void init(Enemy cell)
        {
            this.cell = cell;
            transform.localPosition = cell.position;
            cell.view = this;

            anm.transform.localRotation = Quaternion.Euler(0, flip, 0);
        }


        void IModelView<EnemyMgr>.attach(EnemyMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<EnemyMgr>.detach(EnemyMgr owner)
        {
            this.owner = null;
        }


        void IEnemyView.on_modify_physics_tick()
        {
            transform.localPosition = cell.position;

            anm.transform.localRotation = Quaternion.Euler(0, flip, 0);
        }


        void IModelView<EnemyMgr>.shift(EnemyMgr old_owner, EnemyMgr new_owner)
        {
        }


        void IEnemyView.on_destroy()
        {
            Destroy(gameObject);
        }


        /// <summary>
        /// 临时: 被攻击时外观变红
        /// </summary>
        void IEnemyView.notify_on_damaged()
        {
            //m_sprite.color = Config.current.be_damaged_color;
            //Invoke(nameof(after_damage_view_recover), Config.current.be_damaged_recover_time);
        }


        /// <summary>
        /// 被攻击x秒后, 外观复原
        /// </summary>
        void after_damage_view_recover()
        {
            //m_sprite.color = Color.white;
        }


        /// <summary>
        /// 死亡过程中
        /// </summary>
        void IEnemyView.notify_on_dead_process()
        {
            //m_sprite.flipY = true;
        }
    }
}


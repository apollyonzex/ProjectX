using Foundation;
using UnityEngine;

namespace World_Formal.Enemys.Projectiles
{
    public class ProjectileView : MonoBehaviour, IProjectileView
    {
        public Collider2D collider_box;

        ProjectileMgr mgr;
        public Projectile cell;

        //==================================================================================================

        void IModelView<ProjectileMgr>.attach(ProjectileMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<ProjectileMgr>.detach(ProjectileMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<ProjectileMgr>.shift(ProjectileMgr old_mgr, ProjectileMgr new_mgr)
        {
        }


        void IProjectileView.notify_on_tick1()
        {
            transform.localPosition = cell.view_position;
            transform.localRotation = cell.view_rotation;
        }


        void IProjectileView.notify_on_init(Projectile cell)
        {
            this.cell = cell;
            cell.collider = collider_box;
        }


        void IProjectileView.notify_on_destory()
        {
            DestroyImmediate(gameObject);
        }
    }
}


using Devices;
using Foundation;
using UnityEngine;
using Worlds.Missions.Battles;
using Worlds.Missions.Battles.Projectiles;


namespace DeviceViews
{
    public class ProjectileView : MonoBehaviour, IProjectileView
    {
        Projectile cell;
        ProjectileMgr owner;

        Projectile IProjectileView.cell => this.cell;

        //================================================================================================
        public string rotate_id;
        //================================================================================================


        private void set_direction(Vector2 dir) {
            if (rotate_id!=null&&cell.try_get_component<ProjectileRotate>(rotate_id, out var component)) {
                dir = Quaternion.AngleAxis(component.angle, Vector3.forward) * Vector3.right;
            }
            transform.localRotation = Utility.quick_rotate(dir);
        }


        public void init(Projectile cell)
        {
            this.cell = cell;
            transform.localPosition = cell.position;
            set_direction(cell.direction);
            ///transform.localRotation = Utility.quick_rotate(cell.direction);
        }


        void IProjectileView.on_modify_physics_tick()
        {
            transform.localPosition = cell.position;
            set_direction(cell.direction);
           // transform.localRotation = Utility.quick_rotate(cell.direction);
        }


        void IModelView<ProjectileMgr>.attach(ProjectileMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<ProjectileMgr>.detach(ProjectileMgr owner)
        {
            owner = null; 
        }


        void IModelView<ProjectileMgr>.shift(ProjectileMgr old_owner, ProjectileMgr new_owner)
        {
        }   


        void IProjectileView.on_destory()
        {
            Destroy(gameObject);
        }
    }
}

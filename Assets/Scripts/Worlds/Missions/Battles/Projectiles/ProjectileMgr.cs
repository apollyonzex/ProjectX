using Devices;
using Foundation;
using System.Collections.Generic;


namespace Worlds.Missions.Battles.Projectiles
{
    public interface IProjectileView : IModelView<ProjectileMgr>
    {
        void on_modify_physics_tick();

        void on_destory();

        Projectile cell { get; }
    }


    public class ProjectileMgr : Model<ProjectileMgr, IProjectileView>
    {
        public event System.Action cell_tick;

        public Dictionary<Projectile, IProjectileView> projectiles = new();

        //==================================================================================================


        public void on_physics_tick()
        {
            cell_tick?.Invoke();
            upd_view();
        }


        public void upd_view()
        {
            foreach (var view in views)
            {
                view.on_modify_physics_tick();
            }
        }


        public void destory_cell(Projectile cell)
        {
            var view = projectiles[cell];
            remove_view(view);
            view.on_destory();

            projectiles.Remove(cell);
        }
    }
}


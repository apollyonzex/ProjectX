using Foundation;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.BattleSystem.Device;
using World_Formal.DS;

namespace World_Formal.BattleSystem
{
    public interface IWorldProjectileView : IModelView<WorldProjectile>{
        void tick();
    }
    public class WorldProjectile : Model<WorldProjectile,IWorldProjectileView> ,ITarget
    {
        public  Projectile projectile;

        public bool validate = true;

        public WorldProjectile(Projectile p)
        {
            projectile = p;
        }

        Vector2 ITarget.Position => projectile.position;

        Collider2D ITarget.collider => null;

        public void tick()
        {
            projectile.tick();              //图流tick

            foreach(var view in views)      //视图tick
            {
                view.tick();
            }
        }

        void ITarget.hurt(int dmg)
        {
            projectile.projectile_hurt(dmg);
        }
    }
}

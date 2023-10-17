using UnityEngine;
using Foundation;
using System.Collections.Generic;


namespace World_Formal.BattleSystem
{
    public class WorldProjectileView : MonoBehaviour,IWorldProjectileView
    {
        public WorldProjectile owner;

        void IModelView<WorldProjectile>.attach(WorldProjectile owner)
        {
            this.owner = owner;
        }

        void IModelView<WorldProjectile>.detach(WorldProjectile owner)
        {
            if (this.owner != null)
            {
                this.owner = null;
            }
            Destroy(gameObject);
        }

        void IModelView<WorldProjectile>.shift(WorldProjectile old_owner, WorldProjectile new_owner)
        {
            
        }

        void IWorldProjectileView.tick()
        {
            transform.localPosition = owner.projectile.position;
        }
    }
}

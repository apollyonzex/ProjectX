

using GraphNode;
using System;
using World_Formal.BattleSystem.DeviceGraph.Nodes;
using World_Formal.DS;

namespace World_Formal.BattleSystem.DeviceGraph
{
    public class DeviceContext : IContext
    {
        public Device.GraphDevice device;
        Type IContext.context_type => typeof(DeviceContext);
        
        public DeviceContext(Device.GraphDevice device)
        {
            this.device = device;
        }


        #region GraphAction
        [GraphAction]
        public bool target_damage(ITarget target,[ShowInBody(format =" -[damage] :{0}")] int damage,float knock_back,float knock_dir_x,float knock_dir_y)
        {
            target.hurt(damage);
            return true;
        }

        [GraphAction]
        public bool projectile_confront(ProjectileEvent pe)
        {
            if(pe.target is ITarget t)
            {
                t.hurt(pe.p.desc.f_projectile_dmg);
            }
            return true;
        }
        #endregion
    }

}

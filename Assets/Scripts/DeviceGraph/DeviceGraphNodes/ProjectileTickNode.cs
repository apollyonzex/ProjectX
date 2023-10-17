using System;
using Devices;
using GraphNode;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileTickNode : DeviceComponentNode{

        [Input]
        [Display("projectile")]
        public void get_projectile(DeviceContext ctx, Projectile p) {
            p.add_component(new ProjectileTickComponent(this), true);
        }


        [Output]
        [Display("tick_action")]
        public DeviceEvent<Projectile> action { get; } = new DeviceEvent<Projectile>();


        public void do_action(DeviceContext ctx,Projectile p) {
            action.invoke(ctx, p);
        }
    }
}

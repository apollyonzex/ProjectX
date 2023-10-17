using GraphNode;
using DeviceGraph;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceCollidingProjectileNode : DeviceNode {
        [Input]
        public void collided(DeviceContext ctx, DeviceColliding c) {
            
            if (c.other is Devices.Projectile p) {
                using (ctx.projectile(p)) {
                    self?.invoke(ctx);
                    projectile?.invoke(ctx, p);
                }   
            }
        }


        [Output]
        public DeviceEvent self { get; } = new();
        [Output]
        public DeviceEvent<Devices.Projectile> projectile { get; } = new();
    }
}

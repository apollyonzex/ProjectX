using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CollidingProjectileNode : DeviceNode {
        [Input]
        public void colliding(DeviceContext ctx,ProjectileColliding c) {
            self_action?.invoke(ctx, c.self);
        }

        [Output]
        public DeviceEvent<Projectile> self_action { get; } = new DeviceEvent<Projectile>();
    }
}

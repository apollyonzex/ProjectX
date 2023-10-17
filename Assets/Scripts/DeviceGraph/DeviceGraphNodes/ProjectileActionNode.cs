

using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileActionNode : DeviceNode {

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction<Projectile> method;


        [Input]
        public void invoke(DeviceContext ctx, Projectile projectile) {
            if (method != null) {
                method.invoke(typeof(DeviceContext), ctx, projectile, out var ret);
                if (ret is bool val && val) {
                    _true.invoke(ctx, projectile);
                } else {
                    _false.invoke(ctx, projectile);
                }
            }
        }


        [Output]
        public DeviceEvent<Projectile> _true { get; } = new();

        [Output]
        public DeviceEvent<Projectile> _false { get; } = new();
    }
}

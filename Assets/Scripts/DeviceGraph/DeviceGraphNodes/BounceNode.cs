using CalcExpr;
using DeviceGraph;
using GraphNode;
using UnityEngine;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class BounceNode : DeviceNode {


        [Input]

        public void bounce(DeviceContext ctx, ProjectileColliding c) {
            if(c.self.try_get_component<Devices.ProjectileBounce>("bounce", out var component)) {
                if(component.bounce(ctx, c,out var n)) {
                    if (n == 0) {
                        final?.invoke(ctx, c);
                    }
                }
                return;
            }
            UnityEngine.Debug.Log("缺少bounce初始化组件");
        }
        [Output]
        public DeviceEvent<ProjectileColliding> final { get; set; } = new DeviceEvent<ProjectileColliding>();

    }
}

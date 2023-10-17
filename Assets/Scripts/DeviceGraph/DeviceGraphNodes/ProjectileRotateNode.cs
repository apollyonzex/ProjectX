

using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileRotateNode : DeviceNode {

        [ShowInBody(format = "angle ={0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression angle;

        public string module_id;

        [Input]
        public void add_rotation(DeviceContext ctx,Projectile p) {
            p.add_component(new ProjectileRotate(this), true);
        }
    }
}

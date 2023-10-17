using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class BounceComponentNode : DeviceComponentNode{

        public string name = "bounce";

        [ShowInBody(format = "[max_times] -> {0}")]
        [ExpressionType(ValueType.Integer)]
        public Expression bounce_times;

        [ShowInBody(format = "[coefficient] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public Expression bounce_factor;

        [Input]
        public void init_bounce(DeviceContext ctx,Projectile p) {
            p.add_component(new ProjectileBounce(this,ctx), false);
        }
    }
}

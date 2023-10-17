using CalcExpr;
using Devices;
using GraphNode;


namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceVector2Node : DeviceComponentNode {

        [ShowInBody(format = "[prefeb_part] ->`{0}`")]
        public string module_id;

        [ShowInBody(format = "[init_x] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression init_x;

        [ShowInBody(format = "[init_y] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression init_y;

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            init_x.calc(ctx, typeof(DeviceContext), out float x);
            init_y.calc(ctx, typeof(DeviceContext), out float y);
            ctx.device.add_component(new DeviceVector2(this, x, y),false);
        }
    }
}


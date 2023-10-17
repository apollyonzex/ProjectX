using CalcExpr;
using Devices;
using GraphNode;


namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceFloatNode : DeviceComponentNode {

        [ShowInBody(format = "[prefeb_part] ->`{0}`")]
        public string module_id;

        [ShowInBody(format = "[init_value] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression init_value;

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            init_value.calc(ctx, typeof(DeviceContext), out float value);
            ctx.device.add_component(new DeviceFloat(this, value), false);
        }
    }
}


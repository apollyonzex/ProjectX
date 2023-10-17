using CalcExpr;
using Devices;
using GraphNode;


namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceBoolNode : DeviceComponentNode {

        [ShowInBody(format = "[prefeb_part] ->`{0}`")]
        public string module_id;

        [ShowInBody(format = "[init_value] -> {0}")]
        [ExpressionType(ValueType.Boolean)]
        public DeviceExpression init_value;

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            init_value.calc(ctx, typeof(DeviceContext), out bool value);
            ctx.device.add_component(new DeviceBoolean(this, value), false);
        }
    }
}


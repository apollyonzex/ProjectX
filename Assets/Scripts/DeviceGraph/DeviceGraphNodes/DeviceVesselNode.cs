

using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]

    public class DeviceVesselNode : DeviceComponentNode {

        [ShowInBody(format = "`{0}`")]
        public string module_id;

        public Init init_type;

        [ExpressionType(ValueType.Integer)]
        public Expression custom;

        [ShowInBody(format = "max = {0}" )]
        [ExpressionType(ValueType.Integer)]
        public Expression max;


        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            int value, max_value;
            if (max != null) {
                max.calc(ctx, typeof(DeviceContext), out max_value);
            } else {
                max_value = 1;
            }
            if (init_type == Init.Custom) {
                if (custom != null) {
                    custom.calc(ctx, typeof(DeviceContext), out value);
                } else {
                    value = 0;
                }
            } else {
                value = max_value;
            }
            ctx.device.add_component(new DeviceVessel(this, value, max_value), false);
        }
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceVesselActionNode : DeviceNode {
        [ShowInBody(format = "-> `{0}`")]
        public string module_id;

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction<DeviceVessel> method;

        [Input]
        public void do_action(DeviceContext ctx) {
            if (method != null) {
                if (ctx.device.try_get_component<DeviceVessel>(module_id, out var component)) {
                    method.invoke(typeof(DeviceContext), ctx, component, out var ret);
                    if (ret is bool b && b) {
                        _action.invoke(ctx);
                    }
                }
            }
        }

        [Output]
        public DeviceEvent _action { get; } = new DeviceEvent();
    }

}

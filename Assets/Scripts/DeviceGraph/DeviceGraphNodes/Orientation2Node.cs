

using CalcExpr;
using GraphNode;
using UnityEngine;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Orientation2Node : DeviceComponentNode {

        [ShowInBody(format = "[prefeb_part] -> `{0}`")]
        public string name;

        [ShowInBody(format = "[angular_velocity] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression r_speed;

        [Input]
        public System.Func<DeviceContext, Vector2?> start { get; set; }

        [Input]
        public System.Func<DeviceContext, Vector2?> tick { get; set; }


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            if (ctx.device.try_get_component<Devices.Orientation2>(this, out var component)) {
                return new Vector2 {
                    v = component.direction,
                    normalized = true,
                };
            }
            return null;
        }

        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config) {
            ctx.device.add_component(new Devices.Orientation2(this), true);
        }
    }
}

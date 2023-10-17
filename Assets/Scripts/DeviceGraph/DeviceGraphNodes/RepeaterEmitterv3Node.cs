using Foundation;
using GraphNode;
using UnityEngine;
using Worlds.Missions.Battles;
using CalcExpr;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class RepeaterEmitterv3Node : DeviceComponentNode {

        [ShowInBody(format = "[emit_power] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression power;

        [ShowInBody(format = "[emit_times] -> {0}")]
        [ExpressionType(ValueType.Integer)]
        public DeviceExpression times;

        [ShowInBody(format = "[emit_interval] -> {0}")]
        [ExpressionType(ValueType.Integer)]
        public DeviceExpression intervals;


        [Input]
        [Display("position")]
        public System.Func<DeviceContext, Vector2?> emit_position { get; set; }


        [Input]
        [Display("direction")]
        public System.Func<DeviceContext, Vector2?> direction { get; set; }


        [Input]
        [Display("can_fire")]
        public System.Func<DeviceContext, bool> can_fire { get; set; }


        public void emit(DeviceContext ctx) {

            var pos = emit_position?.Invoke(ctx);
            var dir = direction?.Invoke(ctx);
            if (pos == null || dir == null) {
                return;
            }

            power.calc(ctx, typeof(DeviceContext), out float n);
            var data = new Emitting() {
                position = pos.Value.v,
                direction = dir.Value.get_normalized(),
                init_speed = n,
            };

            emitting?.invoke(ctx, data);
        }
        [Input]
        public void start_fire(DeviceContext ctx) {
            if (can_fire == null) {
                return;
            }
            if (!can_fire.Invoke(ctx)) {
                return;
            }
            if (ctx.device.try_get_component(this, out Devices.EmitTimerComponentv3 component)) {
                component.start_fire(ctx);
            }
        }

        [Output]
        public DeviceEvent<Emitting> emitting { get; } = new DeviceEvent<Emitting>();




        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config) {
            ctx.device.add_component(new Devices.EmitTimerComponentv3(this), true);
        }
    }
}

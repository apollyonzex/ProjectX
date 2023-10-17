using CalcExpr;
using Foundation;
using GraphNode;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceGraph {

    public struct Emitting {
        public UnityEngine.Vector2 position;
        public UnityEngine.Vector2 direction;
        public float init_speed;

    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class EmitterNode : DeviceNode {

        [ShowInBody(format = "[emit_power] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression power;

        [Input]
        [Display("position")]
        public System.Func<DeviceContext, Vector2?> emit_position { get; set; }


        [Input]
        [Display("direction")]
        public System.Func<DeviceContext, Vector2?> direction { get; set; }

        [Input]
        [Display("can_fire")]
        public System.Func<DeviceContext, bool> can_fire { get; set; }

        [Input]
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

        [Output]
        public DeviceEvent<Emitting> emitting { get; } = new DeviceEvent<Emitting>();
    }
}

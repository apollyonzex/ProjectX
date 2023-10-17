
using UnityEngine;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]

    public class MultiEmittingv2Node : DeviceNode {       //连发器

        [ExpressionType(CalcExpr.ValueType.Integer)]

        [ShowInBody(format = "[multi_count] -> {0}")]
        [Display("multi_count")]
        [SortedOrder(1)]
        public DeviceExpression count;

        [ExpressionType(CalcExpr.ValueType.Floating)]
        [ShowInBody(format = "[spread_angle] -> {0}°")]
        [Display("random_spread_angle")]
        [SortedOrder(2)]
        public DeviceExpression spread_angle;

        [Input]
        public void emit(DeviceContext ctx, Emitting e) {
            if (count != null && count.calc(ctx, typeof(DeviceContext), out int n )&& spread_angle.calc(ctx,typeof(DeviceContext),out float angle)) {

                var dir = e.direction;

                for (int i = 0; i < n; ++i) {
                    var rate = Random.Range(-1f, 1f);
                    e.direction = Quaternion.AngleAxis(angle * rate, Vector3.forward) * new Vector3(dir.x, dir.y, 0);
                    output.invoke(ctx, e);
                }
            }
        }

        [Output]
        public DeviceEvent<Emitting> output { get; } = new DeviceEvent<Emitting>();
    }
}

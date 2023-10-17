

using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2BranchNode : DeviceNode {

        [ShowInBody(format = "[output_bool] -> {0}")]
        [ExpressionType(ValueType.Boolean)]
        public DeviceExpression expr;

        [Input]
        [Display("true")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }


        [Input]
        [Display("false")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            expr.calc(ctx, typeof(DeviceContext), out bool ret);
            if (ret) {
                return a?.Invoke(ctx);
            }
            return b?.Invoke(ctx);
        }

    }
}

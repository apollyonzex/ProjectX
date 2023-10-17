using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class BoolBranchNode : DeviceNode {

        [ShowInBody]
        [ExpressionType(ValueType.Boolean)]
        public DeviceExpression b;

        [Input]
        public void input(DeviceContext ctx) {
            b.calc(ctx, typeof(DeviceContext), out bool ret);
            if (ret) {
                _true?.invoke(ctx);
            } else {
                _false?.invoke(ctx);
            }
        }

        [Output]
        public DeviceEvent _true { get; } = new();
        [Output]
        public DeviceEvent _false { get; } = new();
    }
}

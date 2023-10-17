using DeviceGraph;
using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CustomFloatNode : DeviceNode {

        [ShowInBody(format = "[output_bool] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression expr;


        [Output]
        public float output(DeviceContext ctx) {
            if (!expr.calc(ctx, typeof(DeviceContext), out float r)) {
                UnityEngine.Debug.LogWarning("表达式出现问题,默认返回 0");
                return 0;
            }
            return r;
        }
    }
}

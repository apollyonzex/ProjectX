using DeviceGraph;
using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CustomBoolNode : DeviceNode{

        [ShowInBody(format = "[output_bool] -> {0}")]
        [ExpressionType(ValueType.Boolean)]
        public DeviceExpression expr;


        [Output(can_multi_connect = true)]
        public bool output(DeviceContext ctx) {
            if(!expr.calc(ctx, typeof(DeviceContext), out bool r)) {
                UnityEngine.Debug.LogWarning("表达式出现问题,默认返回false");
                return false;   
            }
            return r;
        }
    }
}

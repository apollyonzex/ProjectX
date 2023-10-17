

using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Compare_Float_GreaterNode : DeviceNode {

        [Input]
        public System.Func<DeviceContext, float> value { get; set; }

        [ShowInBody]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression greater_than;


        [Output(can_multi_connect =true)]
        public bool output(DeviceContext ctx) {
            var input_float = value?.Invoke(ctx);
            greater_than.calc(ctx, typeof(DeviceContext), out float expr_float);
            return input_float > expr_float;
        }

    }
}

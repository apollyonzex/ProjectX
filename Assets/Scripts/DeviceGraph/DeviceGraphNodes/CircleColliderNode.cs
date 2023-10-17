

using CalcExpr;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CircleColliderNode : DeviceComponentNode{


        [ExpressionType(ValueType.Floating)]
        public Expression radius;

        [Input]
        [Display("position")]
        public System.Func<DeviceContext, Vector2?> collider_position { get; set; }

    }
}

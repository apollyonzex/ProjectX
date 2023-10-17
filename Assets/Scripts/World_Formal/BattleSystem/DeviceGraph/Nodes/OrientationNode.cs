
using CalcExpr;
using GraphNode;
using World_Formal.BattleSystem.Device;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class OrientationNode : DeviceComponentNode
    {
        [Input]
        public System.Func<DeviceContext, DeviceVector2> target_direction { get; set; }

        [ShowInBody(format = "[r_speed] ->{0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression angular_velocity;

        [Output]
        public DeviceVector2 output(DeviceContext ctx)
        {
            if(ctx.device.try_get_component<OrientationComponent>(this,out var component))
            {
                return new DeviceVector2(component.dir, false);
            }
            return null;
        }

        public override void init(DeviceContext ctx)
        {
            ctx.device.add_component(new OrientationComponent(this), true);
        }
    }
}

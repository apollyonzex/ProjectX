using GraphNode;
using World_Formal.BattleSystem.Device;
using CalcExpr;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class EnemyDetectorNode :DeviceComponentNode
    {
        [ShowInBody(format = "[radius] ->{0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression radius;

        [Input]
        public System.Func<DeviceContext, DeviceVector2> center { get; set; }

        [Output(can_multi_connect = true)]
        public DeviceVector2 output(DeviceContext ctx)
        {
            if(ctx.device.try_get_component(this,out EnemyDetector component))
            {
                var target = component.target;
                if (target != null)
                {
                    return (DeviceVector2)target.Position;
                }
            }
            return null;
        }

        public override void init(DeviceContext ctx)
        {
            ctx.device.add_component(new EnemyDetector(this), true);
        }
    }
}

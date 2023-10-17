using GraphNode;
using World_Formal.BattleSystem.Device;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class PeriodNode : DeviceComponentNode
    {
        [ShowInBody(format = "[interval] -> {0} tick(s)")]
        public int intervals;

        [Output]
        public DeviceEvent output { get; } = new();

        public override void init(DeviceContext ctx)
        {
            ctx.device.add_component(new TimerComponent(this),true);
        }
    }
}

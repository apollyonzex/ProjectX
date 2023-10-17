
using GraphNode;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class PeriodEventNode : DeviceComponentNode {

        [ShowInBody(format = "[interval] -> {0} tick(s)")]
        public int intervals;

        [Output]
        public DeviceEvent output { get; } = new();

        [Output]
        public int out_intervals(DeviceContext ctx)
        {
            return intervals;
        }

        [Output]
        public int out_current_tick(DeviceContext ctx)
        {
            return current_time;
        }

        public int current_time;


        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config) {
            ctx.device.add_component(new Devices.TimerComponent(this), true);
        }

    }
}

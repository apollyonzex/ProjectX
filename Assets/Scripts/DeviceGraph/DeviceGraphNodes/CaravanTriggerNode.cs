using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CaravanTriggerNode : DeviceComponentNode {

        [ShowInBody(format = "'{0}'")]
        public string module_id;

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            ctx.device.add_component(new CaravanTrigger(this), false);
        }

        [Output]
        public DeviceEvent output { get; } = new DeviceEvent();
    }
}

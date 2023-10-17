

using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CaravanDataNode : DeviceComponentNode {

        public string module_id = "caravan";

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            ctx.device.add_component(new CaravanData(this), false);
        }
    }
}

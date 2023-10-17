

using GraphNode;
using UnityEngine;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ConfigPositionNode : DeviceNode {

        [ShowInBody(format = "[prefeb_part] -> `{0}`")]
        public string name;


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            if (ctx.device.try_get_component(this, out Devices.ConfigPosition e)) {
                return (Vector2)e.position;
            }
            return null;
        }

        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config) {
            ctx.device.add_component(new Devices.ConfigPosition(this, config), false);
        }
    }
}

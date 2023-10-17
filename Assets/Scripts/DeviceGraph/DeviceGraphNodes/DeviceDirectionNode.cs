
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceDirectionNode : DeviceNode {

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            return new Vector2 {
                v = ctx.device.direction,
                normalized = true,
            };
        }
    }
}

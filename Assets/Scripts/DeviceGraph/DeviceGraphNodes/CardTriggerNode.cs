using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CardTriggerNode : DeviceComponentNode {

        [ShowInBody(format = "'{0}'")]
        public string module_id;

        private float x, y;

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            ctx.device.add_component(new CardTrigger(this), false);
        }

        public void set_position(UnityEngine.Vector2? v) {
            if (v != null) {
                x = ((UnityEngine.Vector2)v).x;
                y = ((UnityEngine.Vector2)v).y;
            } else {
                x = 0;
                y = 0;
            }
        }

        [Output]
        public DeviceEvent output { get; } = new DeviceEvent();

        [Output(can_multi_connect = true)]
        public Vector2? card_position(DeviceContext ctx) {
            return new Vector2 {
                v = new UnityEngine.Vector2(x, y),
                normalized = false,
            };
        }
    }
}

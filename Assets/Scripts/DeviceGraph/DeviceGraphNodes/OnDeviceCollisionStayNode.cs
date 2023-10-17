

using Devices;
using GraphNode;

namespace DeviceGraph {


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class OnDeviceCollisionStayNode : DeviceComponentNode {

        public string module_id;

        public void stay_collision(DeviceContext ctx, object _other) {
            var ret = condition?.Invoke(ctx);
            if (ret == null || (bool)ret == false) {
                UnityEngine.Debug.Log(ret);
                return;
            }


            collided?.invoke(ctx);

            collided_with_event?.invoke(ctx, new DeviceColliding {
                other = _other,
            });

        }

        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            ctx.device.add_component(new DeviceCollisionStay(this), false);
        }

        [Input]
        public System.Func<DeviceContext, bool> condition { get; set; }

        [Output]
        public DeviceEvent collided { get; } = new DeviceEvent();

        [Output]
        public DeviceEvent<DeviceColliding> collided_with_event { get; } = new DeviceEvent<DeviceColliding>();
    }
}

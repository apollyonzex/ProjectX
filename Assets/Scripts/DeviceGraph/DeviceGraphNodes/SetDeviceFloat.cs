using DeviceGraph;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class SetDeviceFloat : DeviceNode{

        [ShowInBody(format = "`{0}`")]
        public string module_id;

        [Input]
        public void set_float(DeviceContext ctx) {
            var value = float_Value?.Invoke(ctx);
            ctx.device.try_get_component<DeviceFloat>(module_id, out var component);
            if (value == null)
                return;
            component.value = (float)value;
        }

        [Input]
        public System.Func<DeviceContext,float> float_Value { get; set; }
    }
}

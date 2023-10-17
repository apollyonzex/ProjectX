using Devices;
using GraphNode;

namespace DeviceGraph
{

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ConfigSpineActiveNode : DeviceNode {

        [ShowInBody(format = "[prefeb_part] -> `{0}`")]
        public string name;

        //==================================================================================================

        [Output(can_multi_connect = true)]
        public bool output(DeviceContext ctx) {
            ctx.get_device_config(name, out var dc);
            var e = (DeviceConfigSpineActive)dc;
            if (e != null)
            {
                var bl = e.is_active;
                if (bl) e.is_active = false;
                return bl;
            }   
            else
                return false;
        }


        public override void init(DeviceContext ctx, DeviceConfig[] config) {
            config.get_item(name, out DeviceConfigSpineActive dc);
            ctx.set_device_config(name, dc);
        }
    }
}

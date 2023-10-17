using Devices;
using GraphNode;

namespace DeviceGraph
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Config_GetData_IntNode : DeviceNode
    {
        [ShowInBody(format = "[prefeb_part] -> `{0}`")]
        public string name;

        public int tick_order;

        [Input]
        [Display("init_int")]
        public System.Func<DeviceContext, int> init_int { get; set; }

        [Input]
        [Display("int")]
        public System.Func<DeviceContext, int> _int { get; set; }

        //==================================================================================================

        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config)
        {
            ctx.device.add_component(new Devices.Config_GetData_Int_Component(this), true);
            config.get_item(name, out DeviceConfig_GetData_Int dc);
            dc.init_int = init_int.Invoke(ctx);

            ctx.set_device_config(name, dc);
        }


        public void do_config(DeviceContext ctx)
        {
            ctx.get_device_config(name, out var dc);
            (dc as DeviceConfig_GetData_Int)._int = _int.Invoke(ctx);
        }
    }
}

using GraphNode;

namespace DeviceGraph
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class SpineEventNode : DeviceComponentNode
    {
        [Output]
        public DeviceEvent output { get; } = new();

        [Input]
        [Display("is_active")]
        public System.Func<DeviceContext, bool> active_state { get; set; }

        //==================================================================================================

        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config)
        {
            ctx.device.add_component(new Devices.SpineEvent_Component(this), true);
        }
    }
}

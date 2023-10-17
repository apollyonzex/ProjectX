using GraphNode;
namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class TriggerAbleNode : DeviceNode {


        [Input]
        public void trigger(DeviceContext ctx) {
            var b = triggerable?.Invoke(ctx);
            if (b!=null) {
                if ((bool)b) {
                    output?.invoke(ctx);
                }
            }
        }

        [Input]
        [Display("triggerable")]
        public System.Func<DeviceContext, bool> triggerable { get; set; }

        [Output]
        public DeviceEvent output { get; } = new();
    }
}

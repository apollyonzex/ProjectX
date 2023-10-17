
using GraphNode;
namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class NextNode : DeviceNode {

        [Input(can_multi_connect = true)]
        public void next(DeviceContext ctx) {
            output?.invoke(ctx);
        }


        [Output(can_multi_connect = true)]
        public DeviceEvent output { get; } = new DeviceEvent();
    }
}

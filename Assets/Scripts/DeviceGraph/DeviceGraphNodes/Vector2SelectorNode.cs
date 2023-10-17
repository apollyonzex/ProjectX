

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2SelectorNode : DeviceNode {

        [Input]
        [Display("high priority")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }


        [Input]
        [Display("low priority")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            return a?.Invoke(ctx) ?? b?.Invoke(ctx);
        }

    }
}

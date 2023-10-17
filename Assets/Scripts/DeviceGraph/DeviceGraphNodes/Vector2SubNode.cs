

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2SubNode : DeviceNode {

        [Input]
        [Display("to")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }

        [Input]
        [Display("from")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            var a = this.a?.Invoke(ctx);
            var b = this.b?.Invoke(ctx);
            if (a != null && b != null) {
                return (Vector2)(a.Value.v - b.Value.v);
            }
            return null;
        }

    }
}

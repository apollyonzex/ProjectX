

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class PositionToDirectionEventNode : DeviceNode {

        [Input(can_multi_connect = true)]
        public void input(DeviceContext ctx, Vector2? to) {
            if (to != null) {
                var from = this.from?.Invoke(ctx);
                if (from != null) {
                    output.invoke(ctx, new Vector2 {
                        v = to.Value.v - from.Value.v,
                        normalized = false,
                    });
                }
            }
        }

        [Input]
        public System.Func<DeviceContext, Vector2?> from { get; set; }


        [Output]
        public DeviceEvent<Vector2?> output { get; } = new();
    }
}

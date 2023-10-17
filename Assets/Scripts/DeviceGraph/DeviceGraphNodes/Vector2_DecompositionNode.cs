

using GraphNode;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2_DecompositionNode : DeviceNode {

        [Input]
        [Display("raw_vector2")]
        public System.Func<DeviceContext, Vector2?> raw_vec { get; set; }

        [Input]
        [Display("projection_direction")]
        public System.Func<DeviceContext, Vector2?> right { get; set; }

        [Output(can_multi_connect = true)]
        public float output_X(DeviceContext ctx) {
            var a = this.raw_vec?.Invoke(ctx);
            var b = this.right?.Invoke(ctx);
            if (a == null || b == null) {
                return 0;
            }
           
            var vx = b.Value.get_normalized();
            return UnityEngine.Vector2.Dot(a.Value.v,vx);
        }

        [Output(can_multi_connect = true)]
        public float output_Y(DeviceContext ctx) {
            var a = this.raw_vec?.Invoke(ctx);
            var b = this.right?.Invoke(ctx);
            if (a == null || b == null) {
                return 0;
            }
            var vx = b.Value.get_normalized();
            var vy = UnityEngine.Vector2.Perpendicular(vx);
            return UnityEngine.Vector2.Dot(a.Value.v,vy);
        }

    }
}


using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class TransformVector2Node : DeviceNode {

        [Input]
        public System.Func<DeviceContext, Vector2?> value { get; set; }

        [Input]
        public System.Func<DeviceContext, Vector2?> offset { get; set; }

        [Input]
        public System.Func<DeviceContext, Vector2?> right { get; set; }


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            var value = this.value?.Invoke(ctx);
            if (value == null) {
                return null;
            }

            UnityEngine.Vector2 offset;
            if (this.offset == null) {
                offset = UnityEngine.Vector2.zero;
            } else {
                var t = this.offset?.Invoke(ctx);
                if (t == null) {
                    return null;
                }
                offset = t.Value.v;
            }

            var right = this.right?.Invoke(ctx);
            if (right == null) {
                return null;
            }

            var rot = right.Value.get_normalized();

            var v = value.Value.v;

            var x = UnityEngine.Vector2.Dot(v, new UnityEngine.Vector2(rot.x, -rot.y));
            var y = UnityEngine.Vector2.Dot(v, new UnityEngine.Vector2(rot.y, rot.x));

            return (Vector2)(new UnityEngine.Vector2(x, y) + offset);
        }

    }
}

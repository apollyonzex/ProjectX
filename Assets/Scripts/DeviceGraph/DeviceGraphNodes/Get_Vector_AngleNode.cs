
using GraphNode;
namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Get_Vector_AngleNode : DeviceNode {

        [Input]
        [Display("from")]
        [SortedOrder(1)]
        public System.Func<DeviceContext, Vector2?> vector_1 { get; set; }

        [Input]
        [Display("to")]
        [SortedOrder(2)]
        public System.Func<DeviceContext, Vector2?> vector_2 { get; set; }

        [Output(can_multi_connect = true)]
        public float output(DeviceContext ctx) {
            var a = vector_1?.Invoke(ctx);
            var b = vector_2?.Invoke(ctx);
            if (a == null || b == null) {
                UnityEngine.Debug.Log("vector值出现空,默认返回float  0");
                return 0;
            }
            var angle = UnityEngine.Vector2.SignedAngle(a.Value.v, b.Value.v);
            return angle;
        }
    }
}

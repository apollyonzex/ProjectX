
using GraphNode;
namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class BoolAndNode : DeviceNode {

        [Input]
        public System.Func<DeviceContext, bool> boolean_1 { get; set; }

        [Input]
        public System.Func<DeviceContext, bool> boolean_2 { get; set; }

        [Output(can_multi_connect = true)]
        public bool output(DeviceContext ctx) {
            var a = boolean_1?.Invoke(ctx);
            var b = boolean_2?.Invoke(ctx);
            if (a == null || b == null) {
                UnityEngine.Debug.Log("bool值出现空,默认返回false");
                return false;
            }
            return (bool)a && (bool)b;
        }
    }
}

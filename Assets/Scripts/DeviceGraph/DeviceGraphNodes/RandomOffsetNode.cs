using GraphNode;
using UnityEngine;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class RandomOffsetNode : DeviceNode{


        public float offset_angle;

        [Input]
        public System.Func<DeviceContext, Vector2?> origin { get; set; }



        [Output]

        public Vector2? output(DeviceContext ctx) {
            var dir = origin?.Invoke(ctx);
            if (dir != null) {
                var rate = Random.Range(-1f, 1f);
                var v = Quaternion.AngleAxis(offset_angle * rate, Vector3.forward)  * new Vector3(dir.Value.v.x, dir.Value.v.y,0);
                return new Vector2 {
                    v = new UnityEngine.Vector2(v.x, v.y).normalized,
                    normalized = true,
                };
            }
            return null;
        }

    }
}

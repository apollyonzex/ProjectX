using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ColliderDetectorNode :DeviceComponentNode {

        [ShowInBody(format = "[radius] -> {0}")]
        public float radius;

        [Input]
        public void init_collider(DeviceContext ctx, Projectile p) {
            p.add_component(new ColliderDetector(this), true);
        }

    }
}

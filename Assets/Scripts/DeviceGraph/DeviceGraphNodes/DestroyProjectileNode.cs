using Devices;
using GraphNode;

namespace DeviceGraph{


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DestroyProjectileNode  :DeviceNode {

        [Input]
        public void destroy(DeviceContext ctx,Projectile p) {
            p.destroy();
        }
    }
}

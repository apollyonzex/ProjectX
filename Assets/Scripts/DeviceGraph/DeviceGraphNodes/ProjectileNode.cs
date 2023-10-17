using CalcExpr;
using Devices;
using Foundation;
using GraphNode;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceGraph {

    public struct ProjectileColliding {
        public Projectile self;
        public object other;
        public Vector2 normal;
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileNode : DeviceNode {

        [ShowInBody(format = "[resource_bundle] -> `{0}`")]
        [SortedOrder(1)]
        public string bundle;
        [ShowInBody(format = "[resource_path] -> `{0}`")]
        [SortedOrder(2)]
        public string path;


        [Input]
        public void emitting(DeviceContext ctx, Emitting e) {
            Projectile cell = new();
            cell.init(this, ctx ,ref e);
        }


        [Output]
        public DeviceEvent<Projectile> create { get; } = new DeviceEvent<Projectile>();

        [Output]
        public DeviceEvent<ProjectileColliding> collided { get; } = new DeviceEvent<ProjectileColliding>();
    }
}

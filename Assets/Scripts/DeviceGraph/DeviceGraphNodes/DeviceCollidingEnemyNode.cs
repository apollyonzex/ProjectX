using GraphNode;
using DeviceGraph;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceCollidingEnemyNode : DeviceNode {
        [Input]
        public void collided(DeviceContext ctx,DeviceColliding c) {
            if(c.other is Worlds.Missions.Battles.Enemies.Enemy e) {
                self?.invoke(ctx);
                enemy?.invoke(ctx,e);
            }
        }

        
        [Output]
        public DeviceEvent self { get; } = new();
        [Output]
        public DeviceEvent<Worlds.Missions.Battles.Enemies.Enemy> enemy { get; } = new();
    }
}



using GraphNode;
using Worlds.Missions.Battles.Enemies;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class EnemyActionNode : DeviceNode {

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction<Enemy> method;


        [Input]
        public void invoke(DeviceContext ctx, Enemy enemy) {
            if (method != null) {
                method.invoke(typeof(DeviceContext), ctx, enemy, out var ret);
                if (ret is bool val && val) {
                    _true.invoke(ctx, enemy);
                } else {
                    _false.invoke(ctx, enemy);
                }
            }
        }


        [Output]
        public DeviceEvent<Enemy> _true { get; } = new();

        [Output]
        public DeviceEvent<Enemy> _false { get; } = new();
    }
}

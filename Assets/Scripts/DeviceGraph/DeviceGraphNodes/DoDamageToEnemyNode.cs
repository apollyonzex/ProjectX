using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DoDamageToEnemyNode :DeviceNode{


        [ExpressionType(ValueType.Integer)]
        public DeviceExpression damage;

        [Input]

        public void make_damage(DeviceContext ctx, ProjectileColliding c) {                                                      //规定约束,当子弹对抗发生时,永远是碰撞检测的发起者受伤
            if (c.other is Worlds.Missions.Battles.Enemies.Enemy e) {
                damage.calc(ctx, typeof(DeviceContext), out int d);
            }
        }
    }
}
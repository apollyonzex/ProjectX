
using CalcExpr;
using DeviceGraph;
using DeviceViews;
using Worlds.Missions.Battles;

namespace Devices {
    public class CaravanData : DeviceComponent {


        public override DeviceNode graph_node => m_node;
        public override string name => m_node.module_id;

        public CaravanDataNode m_node;
        public CaravanData(CaravanDataNode node) {
            m_node = node;
        }

        [ExprConst("velocity.x")]
        public float speed_x => BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.velocity.x;


        [ExprConst("velocity.y")]
        public float speed_y => BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.velocity.y;

        [ExprConst("driving_speed_limit.value")]
        public float max_speed => BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.driving_speed_limit_readonly;

        [ExprConst("status_acc.value")]
        public int acc_status => (int)BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.acc_status;

        [ExprConst("status_liftoff.value")]
        public int liftoff_status => (int)BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.liftoff_status;
    }
}

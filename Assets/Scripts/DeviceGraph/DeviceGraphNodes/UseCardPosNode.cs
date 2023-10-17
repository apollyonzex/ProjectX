using Devices;
using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class UseCardPosNode : DeviceComponentNode {


        [Output(can_multi_connect = true)]
        public Vector2? card_position(DeviceContext ctx) {
            return (Vector2?)Worlds.Missions.Battles.BattleSceneRoot.instance.battlecardMgr.use_card_position;
        }
    }
}

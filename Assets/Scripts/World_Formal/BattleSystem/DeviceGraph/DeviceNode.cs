
using GraphNode;

namespace World_Formal.BattleSystem.DeviceGraph{

    [System.Serializable]
    public class DeviceNode : Node {

        public virtual void init(DeviceContext ctx) {

        }
    }

    [System.Serializable]
    public class DeviceComponentNode : DeviceNode {

        public int tick_order;

    }
}



using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.DeviceGraph.Nodes;

namespace World_Formal.BattleSystem.Device
{
    public class EmitterComponent : DeviceComponent
    {
        private EmitterNode m_node;
        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;
        public EmitterComponent(EmitterNode node)
        {
            m_node = node;
        }

        public override void tick(DeviceContext ctx)
        {

        }
    }
}

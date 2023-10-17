using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.DeviceGraph.Nodes;

namespace World_Formal.BattleSystem.Device
{
    public class TimerComponent : DeviceComponent
    {
        private PeriodNode m_node;

        public override DeviceNode graph_node => m_node;

        public override int tick_order => m_node.tick_order;

        private int current;
        public int interval;

        public TimerComponent(PeriodNode node)
        {
            m_node = node;
            current = 0;
            interval = node.intervals;
        }

        public override void tick(DeviceContext ctx)
        {
            current++;
            if(current > interval)
            {
                current -= interval;
                m_node.output?.invoke(ctx);
            }

        }
    }
}

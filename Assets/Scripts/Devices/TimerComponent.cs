

using DeviceGraph;

namespace Devices {
    public class TimerComponent :DeviceComponent{

        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        private int current_time = 0 ;

        public TimerComponent(PeriodEventNode node) {
            m_node = node;
            current_time = m_node.intervals;
        }

        public override void tick(DeviceContext ctx) {
            current_time--;
            if (current_time <= 0) {
                current_time = m_node.intervals;
                m_node.output.invoke(ctx);
            }

            m_node.current_time = current_time;
            m_node.out_current_tick(ctx);
        }

        private PeriodEventNode m_node;
    }
}

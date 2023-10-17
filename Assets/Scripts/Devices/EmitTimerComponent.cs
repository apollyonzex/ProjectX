

using DeviceGraph;

namespace Devices {
    public class EmitTimerComponent : DeviceComponent {

        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        private int current_time = 0;

        private int current_count = 0;

        private int times;
        private int interval;

        public EmitTimerComponent(RepeaterEmitterNode node) {
            m_node = node;
        }

        public override void tick(DeviceContext ctx) {

            if (current_count == 0)
                return;

            current_time--;
            while (current_time <= 0) {
                current_time = interval;
                current_count--;
                m_node.emit(ctx);
                if (current_count <= 0) {
                    break;
                }
            }
        }

        public void start_fire(DeviceContext ctx) {
            m_node.intervals.calc(ctx, typeof(DeviceContext), out int interval);
            m_node.times.calc(ctx, typeof(DeviceContext), out int times);
            this.times = times;
            this.interval = interval;
            current_count =this.times;
            current_time = 0;
        }


        private RepeaterEmitterNode m_node;
    }
}

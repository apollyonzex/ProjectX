

using DeviceGraph;

namespace Devices {
    public class CaravanTrigger : DeviceComponent {

        private CaravanTriggerNode m_node;

        public CaravanTrigger(CaravanTriggerNode node) {
            m_node = node;
        }

        public override string name => m_node.module_id;
        public override DeviceNode graph_node => m_node;

        public void trigger(DeviceContext ctx) {
            m_node.output?.invoke(ctx);
        }
    }
}

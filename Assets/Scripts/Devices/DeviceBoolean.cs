

using CalcExpr;
using DeviceGraph;
using DeviceViews;

namespace Devices {
    public class DeviceBoolean : DeviceComponent, IActiveProvider {

        private DeviceBoolNode m_node;
        private bool m_value;
        public override string name => m_node.module_id;
        public override DeviceNode graph_node => m_node;
        object[] IProvider.component_return_prms => null;


        [ExprConst]
        public bool value {
            get => m_value;
            set => m_value = value;
        }


        public DeviceBoolean(DeviceBoolNode node,bool init) {
            m_value = init;
            m_node = node;
        }
    }
}

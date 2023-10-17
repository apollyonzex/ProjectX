

using CalcExpr;
using DeviceGraph;
using DeviceViews;

namespace Devices {
    public class DeviceFloat : DeviceComponent ,IProgressProvider{ 
        private DeviceFloatNode m_node;
        private float m_value;
        public override string name => m_node.module_id;
        public override DeviceNode graph_node => m_node;
        object[] IProvider.component_return_prms => null;


        [ExprConst]
        public float value {
            get => m_value;
            set => m_value = value;
        }

        public float progress => value;


        public DeviceFloat(DeviceFloatNode node, float init) {
            m_value = init;
            m_node = node;
        }
    }
}

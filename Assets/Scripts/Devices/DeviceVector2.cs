

using CalcExpr;
using DeviceGraph;
using DeviceViews;

namespace Devices {
    public class DeviceVector2 : DeviceComponent, IVector2Provider {
        private DeviceVector2Node m_node;
        private float m_x;
        private float m_y;
        public override string name => m_node.module_id;
        public override DeviceNode graph_node => m_node;
        object[] IProvider.component_return_prms => null;


        [ExprConst]
        public float x {
            get => m_x;
            set => m_x = value;
        }

        [ExprConst]
        public float y {
            get => m_y;
            set => m_y = value;
        }


        public DeviceVector2(DeviceVector2Node node, float init_x,float init_y) {
            m_x = init_x;
            m_y = init_y;
            m_node = node;
        }
    }
}

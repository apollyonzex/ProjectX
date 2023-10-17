

using DeviceGraph;

namespace Devices {
    public class ConfigPosition : DeviceComponent {

        public ConfigPosition(ConfigPositionNode node, DeviceConfig[] config) {
            m_node = node;
            if (config.get_item(node.name, out DeviceConfigPosition e)) {
                m_position = e.get_position();
            }
        }

        public UnityEngine.Vector2 position => m_position;

        public override DeviceNode graph_node => m_node;


        private ConfigPositionNode m_node;
        private UnityEngine.Vector2 m_position;
    }
}

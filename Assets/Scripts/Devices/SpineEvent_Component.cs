using DeviceGraph;
using GraphNode;
using Spine;

namespace Devices
{
    public class SpineEvent_Component : DeviceComponent
    {
        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        SpineEventNode m_node;

        //==================================================================================================

        public SpineEvent_Component(SpineEventNode node)
        {
            m_node = node;
        }

        public override void tick(DeviceContext ctx)
        {
            if (m_node.active_state(ctx))
                m_node.output.invoke(ctx);
        }
    }
}



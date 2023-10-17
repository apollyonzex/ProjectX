using DeviceGraph;

namespace Devices
{
    public class Config_GetData_Int_Component : DeviceComponent
    {
        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        Config_GetData_IntNode m_node;

        //==================================================================================================

        public Config_GetData_Int_Component(Config_GetData_IntNode node)
        {
            m_node = node;
        }


        public override void tick(DeviceContext ctx)
        {
            m_node.do_config(ctx);
        }
    }
}



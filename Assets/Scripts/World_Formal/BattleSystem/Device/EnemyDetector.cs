
using Common_Formal;
using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.DeviceGraph.Nodes;
using World_Formal.DS;

namespace World_Formal.BattleSystem.Device
{
    public class EnemyDetector : DeviceComponent
    {
        private ITarget m_target;
        public ITarget target => m_target;

        public override int tick_order => m_node.tick_order;
        public override DeviceNode graph_node => m_node;

        private EnemyDetectorNode m_node;
        public EnemyDetector(EnemyDetectorNode node)
        {
            m_node = node;
        }

        public override void tick(DeviceContext ctx)
        {
            var pos = m_node.center?.Invoke(ctx);
            if (pos != null)
            {
                m_node.radius.calc(ctx, typeof(DeviceContext), out float r);
                var new_target = BattleUtility.select_target_in_circle(pos.v, r, Enum.EN_faction.player);
                if (new_target != m_target)
                {
                    m_target = new_target;
                }
            }
        }
    }
}


using Worlds.Missions.Battles;
using DeviceGraph;

namespace Devices {
    public class ProjectileDetector : DeviceComponent{

        public override DeviceNode graph_node => m_node;

        public override int tick_order => m_node.tick_order;

        public ProjectileDetector(ProjectileDetectorNode node) {
            m_node = node;
        }

        public override void tick(DeviceContext ctx) {
            var pos = m_node.center?.Invoke(ctx);
            if (pos != null) {
                m_node.radius.calc(ctx,typeof(DeviceContext),out float n);
                var new_target = Utility.select_target_in_circle<Worlds.Missions.Battles.Projectiles.IProjectileView>(pos.Value.v, n, ctx.device.faction);
                if (new_target != m_target) {
                    if (m_target != null) {
                        if (new_target == null) {
                            var t = m_target;
                            m_target = null;
                            m_node.target_leave.invoke(ctx, t);
                        } else {
                            m_target = new_target;
                            m_node.target_stay.invoke(ctx, m_target);
                        }
                    } else if (new_target != null) {
                        m_target = new_target;
                        m_node.target_enter.invoke(ctx, m_target);
                    }
                } else if (m_target != null) {
                    m_node.target_stay.invoke(ctx, m_target);
                }
            }
        }

        public ITarget target => m_target;
        private ITarget m_target;
        private ProjectileDetectorNode m_node;
    }
}

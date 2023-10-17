using DeviceGraph;

namespace Devices {
    public class GradientProjectileMovement : ProjectileComponent {

        public override DeviceNode graph_node => m_node;

        public GradientProjectileMovement(GradientMovementNode node) {
            m_node = node;
        }

        public override void tick(DeviceContext ctx, Projectile p) {
            m_node.move(ctx, p);
        }

        private GradientMovementNode m_node;

    }
}
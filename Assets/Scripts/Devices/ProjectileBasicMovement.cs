using DeviceGraph;

namespace Devices {
    public class ProjectileBasicMovement : ProjectileComponent {

        public override DeviceNode graph_node => m_node;

        public ProjectileBasicMovement(MovementNode node) {
            m_node = node;
        }

        public override void tick(DeviceContext ctx,Projectile p) {
            m_node.move(ctx,p);
        }

        private MovementNode m_node;

    }
}

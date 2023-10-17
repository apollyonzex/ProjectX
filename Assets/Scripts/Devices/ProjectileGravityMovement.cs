using DeviceGraph;

namespace Devices {
    public class ProjectileGravityMovement : ProjectileComponent {

        public override DeviceNode graph_node => m_node;

        public ProjectileGravityMovement(MovementWithGravityNode node) {
            m_node = node;
        }

        public override void tick(DeviceContext ctx, Projectile p) {
            using (ctx.projectile(p)) {
                m_node.move(ctx, p);
            }
        }

        private MovementWithGravityNode m_node;

    }
}

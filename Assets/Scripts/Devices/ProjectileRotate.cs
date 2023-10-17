

using DeviceGraph;

namespace Devices {
    public class ProjectileRotate : ProjectileComponent {

        private ProjectileRotateNode m_node;

        public override string name => m_node.module_id;

        public override DeviceNode graph_node => m_node;

        public float angle;


        public override void tick(DeviceContext ctx, Projectile p) {
            using (ctx.projectile(p)) {
                m_node.angle.calc(ctx, typeof(DeviceContext), out this.angle);
            }
        } 

        public ProjectileRotate(ProjectileRotateNode node) {
            m_node = node;
        }
    }
}

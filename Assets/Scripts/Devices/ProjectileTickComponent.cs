using DeviceGraph;
using UnityEngine;

namespace Devices {
    public class ProjectileTickComponent : ProjectileComponent {
        private ProjectileTickNode m_node;

        public ProjectileTickComponent(ProjectileTickNode node) {
            m_node = node;
        }
        public override void tick(DeviceContext ctx, Projectile p) {
            using (ctx.projectile(p)) {
                m_node.do_action(ctx, p);
            }
        }
    }
}

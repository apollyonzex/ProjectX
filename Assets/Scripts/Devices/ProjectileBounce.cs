using DeviceGraph;

namespace Devices {
    public class ProjectileBounce : ProjectileComponent {
        public override string name => m_node.name;
        public override DeviceNode graph_node => m_node;

        private int times;
        public ProjectileBounce(BounceComponentNode node,DeviceContext ctx) {
            m_node = node;
            m_node.bounce_times.calc(ctx, typeof(DeviceContext), out int n);
            times = n;
        }


        public bool bounce(DeviceContext ctx, ProjectileColliding c,out int remains) {

            m_node.bounce_factor.calc(ctx, typeof(DeviceContext), out float factor);

            if (c.other is Floor f) {
                var v = c.self.projectile_velocity;
                if (v.normalized.y > 0) {
                    remains = times;
                    return false;
                }
                times--;
                var v1 = c.self.caravan_velocity;
                c.self.projectile_velocity = new UnityEngine.Vector2(v.x, -v.y * factor);
                c.self.caravan_velocity = new UnityEngine.Vector2(v1.x, -v1.y * factor);
                var vf = c.self.projectile_velocity + c.self.caravan_velocity;
                c.self.set_m_velocity(vf);
                c.self.set_m_direction(vf.normalized);  //重新设置速度和方向
                remains = times;
                return true;
            }
            remains = times;
            return false;
        }

        private BounceComponentNode m_node;

    }
}


using CalcExpr;
using DeviceGraph;

namespace Devices {
    public class DeviceCollisionStay : DeviceComponent {

        public override DeviceNode graph_node => m_node;
        public override string name => m_node.module_id;


        private OnDeviceCollisionStayNode m_node;

        public DeviceCollisionStay(OnDeviceCollisionStayNode node) {
            m_node = node;
        }

        public void stay_collision(Device device, object other) {
            if (other is Worlds.Missions.Battles.Enemies.IEnemyView ev) {
                var normal = new Vector2 {
                    v = ev.cell.position - device.position,
                    normalized = false,
                };
                using (device.ctx.enemy(ev.cell)) using (device.ctx.normal(normal)) {
                    m_node.stay_collision(device.ctx, ev.cell);
                }
            } else if (other is Worlds.Missions.Battles.Projectiles.IProjectileView pv) {
                var normal = new Vector2 {
                    v = pv.cell.velocity - device.position,
                    normalized = false,
                };
                using (device.ctx.projectile(pv.cell)) using (device.ctx.normal(normal)) {
                    m_node.stay_collision(device.ctx, pv.cell);
                }
            }
        }
    }
}

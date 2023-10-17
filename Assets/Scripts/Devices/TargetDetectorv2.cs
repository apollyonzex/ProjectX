

using DeviceGraph;
using UnityEngine;
using Worlds.Missions.Battles;

namespace Devices {


    public class TargetDetectorv2 : DeviceComponent {

        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        public TargetDetectorv2(TargetDetectorNodev2 node) {
            m_node = node;
        }


        public override void tick(DeviceContext ctx) {
            var pos = m_node.center?.Invoke(ctx);
            if (pos != null) {
                var radius = m_node.radius;
                var new_target = Utility.select_target_in_circle(pos.Value.v, radius, ctx.device.faction);
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

        private TargetDetectorNodev2 m_node;
        private ITarget m_target;
    }
}



using DeviceGraph;
using UnityEngine;
using Worlds.Missions.Battles;

namespace Devices {


    public class TargetDetector_Fan : DeviceComponent {
        public override DeviceNode graph_node => m_node;
        public override int tick_order => m_node.tick_order;

        public TargetDetector_Fan(TargetDetector_FanNode node) {
            m_node = node;
        }


        public override void tick(DeviceContext ctx) {
            var pos = m_node.center?.Invoke(ctx);
            var axis = m_node.axis?.Invoke(ctx);
            if (pos != null && axis!=null) {
                m_node.angle.calc(ctx, typeof(DeviceContext), out float angle);
                m_node.radius.calc(ctx, typeof(DeviceContext), out float radius);
                var new_target = Utility.select_target_in_fan(pos.Value.v, radius,axis.Value.v,angle, ctx.device.faction);
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

        private TargetDetector_FanNode m_node;
        private ITarget m_target;
    }
}

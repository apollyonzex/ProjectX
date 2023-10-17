
using DeviceGraph;
using DeviceViews;
using UnityEngine;

namespace Devices {
    public class Orientation : DeviceComponent, IDirectionProvider {

        public Orientation(OrientationNode node) {
            m_node = node;
        }

        public override DeviceNode graph_node => m_node;
        public override string name => m_node.name;
        public override int tick_order => m_node.tick_order;
        public UnityEngine.Vector2 direction => m_direction;

        object[] IProvider.component_return_prms => null;

        public override void start(DeviceContext ctx) {
            var dir = m_node.start?.Invoke(ctx);
            if (dir != null) {
                m_direction = dir.Value.get_normalized();
            }
        }

        public override void tick(DeviceContext ctx) {
            var dir = m_node.tick?.Invoke(ctx);
            if (dir != null) {

                var td = dir.Value.get_normalized();

                var degree = UnityEngine.Vector2.SignedAngle(m_direction, td);

                UnityEngine.Vector3 new_vec;
                if (Mathf.Abs(degree) < m_node.r_speed) {
                    new_vec = Quaternion.AngleAxis(degree, Vector3.forward) * m_direction;
                } else {
                    var r_degree = m_node.r_speed;
                    if (degree > 0) {
                        new_vec = Quaternion.AngleAxis(r_degree, Vector3.forward) * m_direction;
                    } else {
                        new_vec = Quaternion.AngleAxis(-r_degree, Vector3.forward) * m_direction;
                    }
                }
                m_direction = new_vec;
      
                UnityEngine.Debug.DrawRay(ctx.device.position, m_direction * 5, UnityEngine.Color.magenta);
            }


        }

        private UnityEngine.Vector2 m_direction;

        private OrientationNode m_node;
        
    }
}

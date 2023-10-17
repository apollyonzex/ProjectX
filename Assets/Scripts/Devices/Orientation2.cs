
using DeviceGraph;
using DeviceViews;
using UnityEngine;

namespace Devices {
    public class Orientation2 : DeviceComponent, IDirectionProvider {

        public Orientation2(Orientation2Node node) {
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
                m_node.r_speed.calc(ctx, typeof(DeviceContext),out float r_speed);
                if (Mathf.Abs(degree) < r_speed) {
                    new_vec = Quaternion.AngleAxis(degree, Vector3.forward) * m_direction;
                } else {
                    var r_degree = r_speed;
                    if (degree > 0) {
                        new_vec = Quaternion.AngleAxis(r_degree, Vector3.forward) * m_direction;
                    } else {
                        new_vec = Quaternion.AngleAxis(-r_degree, Vector3.forward) * m_direction;
                    }
                }
                m_direction = new_vec;
            }


        }

        private UnityEngine.Vector2 m_direction;

        private Orientation2Node m_node;

    }
}

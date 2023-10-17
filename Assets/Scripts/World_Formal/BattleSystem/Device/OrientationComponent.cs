using Common_Formal;
using UnityEngine;
using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.DeviceGraph.Nodes;

namespace World_Formal.BattleSystem.Device
{
    public class OrientationComponent : DeviceComponent
    {
        private OrientationNode m_node;

        public override DeviceNode graph_node => m_node;

        public override int tick_order => m_node.tick_order;

        public Vector2 dir;
        private Vector2 m_dir;

        public OrientationComponent(OrientationNode node)
        {
            m_node = node;
        }

        public override void tick(DeviceContext ctx)
        {
            var target_direction = m_node.target_direction?.Invoke(ctx);
            var direction = ctx.device.device.direction;
            if (target_direction != null)
            {
                var td = target_direction.get_normalized();
                var degree = Vector2.SignedAngle(m_dir, td);

                Vector3 new_vec;
                m_node.angular_velocity.calc(ctx, typeof(DeviceContext), out float r_speed);
                if(Mathf.Abs(degree) < r_speed)
                {
                    new_vec = Quaternion.AngleAxis(degree, Vector3.forward) * m_dir;
                }
                else
                {
                    if(degree > 0)
                    {
                        new_vec = Quaternion.AngleAxis(r_speed, Vector3.forward) * m_dir;
                    }
                    else
                    {
                        new_vec = Quaternion.AngleAxis(-r_speed, Vector3.forward) * m_dir;
                    }
                }
                m_dir = new_vec;

                ctx.device.device.direction = m_dir;
            }
        }
    }
}

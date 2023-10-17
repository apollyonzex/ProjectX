using Devices;
using GraphNode;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class GradientMovementNode : DeviceNode{
        
        public float lower_gradient;

        public float upper_gradient;

        
        [Input]
        public void init_movement(DeviceContext ctx ,Projectile p) {
            p.add_component(new GradientProjectileMovement(this),true);
        }

        public void move(DeviceContext ctx,Projectile p) {
            if (p.freeze == true) {
                p.freeze = false;
                return;
            }

            float gradient = Random.Range(lower_gradient, upper_gradient);

            p.projectile_velocity *= (1 - gradient);
            
            var pv = p.projectile_velocity;
            var cv = p.caravan_velocity;
            
            var ve = pv + cv;

            var dir = ve.normalized;

            p.set_m_velocity(ve);
            p.set_m_position(p.position + new UnityEngine.Vector2(p.velocity.x, p.velocity.y) * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);
            p.set_m_direction(dir);
        }
    }
}

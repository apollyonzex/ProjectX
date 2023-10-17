
using Devices;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class MovementWithGravityNode : DeviceNode {

        [ShowInBody(format = "[gravity] -> {0}")]
        public float g;
        [Input]
        public void init_movement(DeviceContext ctx, Projectile p) {
            p.add_component(new ProjectileGravityMovement(this), true);
        }

        public void move(DeviceContext ctx, Projectile p) {
            if (p.freeze == true) {
                p.freeze = false;
                return;
            }

            var pv = p.projectile_velocity;
            p.projectile_velocity = new UnityEngine.Vector2(pv.x, pv.y + g * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);
            var ve = p.projectile_velocity + p.caravan_velocity;         //*1/60

            

            var dir = p.projectile_velocity.normalized;
            p.set_m_velocity(ve);
            p.set_m_position(p.position + new UnityEngine.Vector2(p.velocity.x, p.velocity.y) * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);
            p.set_m_direction(dir);
        }
    }




}

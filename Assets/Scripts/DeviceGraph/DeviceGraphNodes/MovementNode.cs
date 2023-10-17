
using Devices;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class MovementNode : DeviceNode {
        [Input]
        public void init_movement(DeviceContext ctx,Projectile p) {
            p.add_component(new ProjectileBasicMovement(this), true);
        }

        public void move(DeviceContext ctx,Projectile p) {
            if (p.freeze == true) {
                p.freeze = false;
                return;
            }
            var v = p.velocity;
            p.set_m_position(p.position + new UnityEngine.Vector2(p.velocity.x, p.velocity.y) * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);
        }
    }




}

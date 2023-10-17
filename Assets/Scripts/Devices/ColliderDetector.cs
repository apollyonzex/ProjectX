using DeviceGraph;
using System.Collections.Generic;
using Worlds.Missions.Battles;


namespace Devices {
    public class ColliderDetector : ProjectileComponent {
        public override DeviceNode graph_node => m_node;

        public override void tick(DeviceContext ctx, Projectile p) {
            var targets = Utility.select_all_target_in_circle(p.position, m_node.radius,ctx.device.faction);
            for (int i = 0; i < hit_colliders.Count; i++) {
                hit_colliders[i] = (hit_colliders[i].target, false);
            }
            foreach (var target in targets) {
                if (!insert_target(target)) {
                    p.notify_collided(ctx, target, p.position - target.position);
                }
            }

            for (int i = 0; i < hit_colliders.Count; ) {
                if (!hit_colliders[i].stay) {
                    var last = hit_colliders.Count - 1;
                    if (i != last) {
                        hit_colliders[i] = hit_colliders[last];
                    }
                    hit_colliders.RemoveAt(last);
                } else {
                    ++i;
                }
            }
            
        }



        private List<(ITarget target, bool stay)> hit_colliders = new List<(ITarget, bool)>();
        
        private bool insert_target(ITarget target) {
            for (int i = 0, c = hit_colliders.Count; i < c; ++i) {
                if (hit_colliders[i].target == target) {
                    hit_colliders[i] = (target, true);
                    return true;
                }
            }
            hit_colliders.Add((target, true));
            return false;
        }

        public ColliderDetector(ColliderDetectorNode node) {
            m_node = node;
        }
        
        private ColliderDetectorNode m_node;
    }
}

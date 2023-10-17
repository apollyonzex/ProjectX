using Devices;
using DeviceViews;
using GraphNode;
using Worlds.Missions.Battles.Enemies;
using Worlds.Missions.Battles.Projectiles;


namespace DeviceGraph
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CollisionNode : DeviceNode
    {
        [Input]
        public void collided(DeviceContext ctx, ProjectileColliding c)
        {
            if (c.other is Enemy e)
            {
                /*e.current_hp -= c.self.node.default_damage;
                if (e.current_hp > 0) return;

                e.destroy();
                UnityEngine.Debug.Log($"已击杀怪物");*/
            }
        }

    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CollisionWithProjectile : DeviceNode
    {
        [Input]
        public void collided(DeviceContext ctx, ProjectileColliding c)
        {
            if (c.other is Projectile p)
            {                                
                self_events?.invoke(ctx, c.self);
                other_events?.invoke(ctx, p);
            }

        }

        [Output]
        [Display("self")]
        public DeviceEvent<Projectile> self_events { get; } = new DeviceEvent<Projectile>();

        [Output]
        [Display("other")]
        public DeviceEvent<Projectile> other_events { get; } = new DeviceEvent<Projectile>();

    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CollisionWithEnviroment : DeviceNode
    {
        [Input]
        public void collided(DeviceContext ctx, ProjectileColliding c)
        {
            if (c.other is Floor)
            {         //other is Enviroment
                self_events?.invoke(ctx, c);
            }
        }

        [Output]
        [Display("self")]
        public DeviceEvent<ProjectileColliding> self_events { get; } = new DeviceEvent<ProjectileColliding>();
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CollisionWithEnemyNode : DeviceNode {
        [Input]
        public void collided(DeviceContext ctx, ProjectileColliding c) {
            if (c.other is Enemy e) {
                using(ctx.enemy(e)) using(ctx.normal(c.normal)) {
                    enemy.invoke(ctx, e);
                    self.invoke(ctx, c.self);
                }
            }
        }

        [Output]
        public DeviceEvent<Enemy> enemy { get; } = new();

        [Output]
        public DeviceEvent<Projectile> self { get; } = new();
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CurrentNormalNode : DeviceNode {

        [Output]
        public Vector2? output(DeviceContext ctx) {
            return ctx.current_normal;
        }
    }
}

using CalcExpr;
using Common;
using DeviceGraph;
using System.Collections.Generic;
using Worlds;
using Worlds.Missions.Battles;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Projectiles;


namespace Devices
{

    public class ProjectileComponent
    {

        public virtual string name => null;
        public virtual DeviceNode graph_node => null;
        public virtual void tick(DeviceContext ctx, Projectile p)
        {

        }
    }

    public class Projectile : ITarget {
        private List<ProjectileComponent> m_components = new List<ProjectileComponent>();
        private List<ProjectileComponent> m_components_with_tick = new List<ProjectileComponent>();
        private Dictionary<string, List<ProjectileComponent>> m_component_name_indices = new Dictionary<string, List<ProjectileComponent>>();
        private Dictionary<DeviceNode, ProjectileComponent> m_component_node_indices = new Dictionary<DeviceNode, ProjectileComponent>();

        public ProjectileNode node => m_node;
        private ProjectileNode m_node;

        [ExprConst("position.x")]
        public float px => m_positon.x;

        [ExprConst("position.y")]
        public float py => m_positon.y;
        public UnityEngine.Vector2 position => m_positon;
        public UnityEngine.Vector2 velocity => m_velocity;

        public UnityEngine.Vector2 projectile_velocity;

        public UnityEngine.Vector2 caravan_velocity;            //记录的是生成那一时刻的车速
        public UnityEngine.Vector2 direction => m_direction;

        public bool freeze = true;

        private UnityEngine.Vector2 m_positon;
        private UnityEngine.Vector2 m_velocity;
        private UnityEngine.Vector2 m_direction;

        ProjectileMgr mgr;

        DeviceContext m_ctx;
        public DeviceContext ctx => m_ctx;

        //==================================================================================================


        public void destroy()
        {
            var e = mgr.projectiles;
            if (!e.ContainsKey(this)) return;

            mgr.cell_tick -= tick;
            BattleCaravanMgr.reset_x -= on_reset_x;

            mgr.destory_cell(this);            
        }


        public void init(ProjectileNode node, DeviceContext ctx, ref Emitting e)
        {
            m_node = node;
            mgr = WorldState.instance.mission.battleMgr.projectile_mgr;
            m_ctx = ctx;

            m_positon = e.position;
            m_direction = e.direction * e.init_speed;
            projectile_velocity = e.direction * e.init_speed;
            caravan_velocity = ctx.device.velocity;
            m_velocity = e.direction * e.init_speed + ctx.device.velocity;     //demo only

            BattleSceneRoot.instance.create_projectile_view(mgr, this, node.bundle, node.path, out var view);

            using (ctx.projectile(this)) {
                m_node.create.invoke(ctx, this);
            }
            mgr.cell_tick += tick;
            BattleCaravanMgr.reset_x += on_reset_x;
        }


        public void tick()
        {
            foreach (var component in m_components_with_tick)
            {
                component.tick(m_ctx, this);
            }
        }


        public void on_reset_x()
        {
            m_positon = new UnityEngine.Vector2(m_positon.x - Config.current.reset_pos_intervel, m_positon.y);
        }


        public void notify_collided(DeviceContext ctx, ITarget target, UnityEngine.Vector2 toward)
        {
            using (ctx.projectile(this)) {
                m_node.collided.invoke(ctx, new ProjectileColliding {
                    self = this,
                    other = target,
                    normal = new Vector2 {
                        v = toward,
                        normalized = false,
                    }
                });
            }
        }

        public void set_m_velocity(UnityEngine.Vector2 _new)
        {
            m_velocity = _new;
        }

        public void set_m_position(UnityEngine.Vector2 _new)
        {
            m_positon = _new;
        }

        public void set_m_direction(UnityEngine.Vector2 _new)
        {
            m_direction = _new;
        }

        public void add_component(ProjectileComponent component, bool need_tick)
        {
            if (need_tick)
            {
                m_components_with_tick.Add(component);
            }
            else
            {
                m_components.Add(component);
            }
            var name = component.name;

            if (name != null)
            {
                if (!m_component_name_indices.TryGetValue(name, out var list))
                {
                    list = new List<ProjectileComponent>();
                    m_component_name_indices.Add(name, list);
                }
                list.Add(component);
            }

            var node = component.graph_node;
            if (node != null)
            {
                m_component_node_indices.Add(node, component);
            }

        }
        public bool try_get_component<T>(string name, out T component) where T : ProjectileComponent {
            if (m_component_name_indices.TryGetValue(name, out var list)) {
                foreach (var e in list) {
                    if (e is T t) {
                        component = t;
                        return true;
                    }
                }
            }
            component = null;
            return false;
        }

        public bool try_get_component(System.Type type, string name, out ProjectileComponent component) {
            if (m_component_name_indices.TryGetValue(name, out var list)) {
                foreach (var e in list) {
                    if (type.IsAssignableFrom(e.GetType())) {
                        component = e;
                        return true;
                    }
                }
            }
            component = null;
            return false;
        }

        public bool try_get_component(DeviceNode node,out ProjectileComponent component) {
            return m_component_node_indices.TryGetValue(node, out component);
        }

        public bool try_get_component<T>(DeviceNode node, out T component) where T : ProjectileComponent {
            if (m_component_node_indices.TryGetValue(node, out var e) && e is T t) {
                component = t;
                return true;
            }
            component = null;
            return false;
        }

    }
}

using UnityEngine;
using System.Collections.Generic;
using World_Formal.BattleSystem.DeviceGraph;
using Common_Formal;

namespace World_Formal.BattleSystem.Device
{
    public class ProjectileComponent
    {
        public virtual string name => null;
        public virtual DeviceNode graph_node => null;

        public virtual void tick(DeviceContext ctx,Projectile p)
        {

        }
    }

    public class Projectile
    {

        public AutoCode.Tables.Projectile.Record desc;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 direction;
        public Enum.EN_faction faction;
        public int current_hp;


        public bool freeze = true;
        public DeviceContext context => m_context;
        private DeviceContext m_context;

        private List<ProjectileComponent> m_components = new List<ProjectileComponent>();
        private List<ProjectileComponent> m_components_with_tick = new List<ProjectileComponent>();
        private Dictionary<string, List<ProjectileComponent>> m_component_name_indices = new Dictionary<string, List<ProjectileComponent>>();
        private Dictionary<DeviceNode, ProjectileComponent> m_component_node_indices = new Dictionary<DeviceNode, ProjectileComponent>();

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
        public bool try_get_component<T>(string name, out T component) where T : ProjectileComponent
        {
            if (m_component_name_indices.TryGetValue(name, out var list))
            {
                foreach (var e in list)
                {
                    if (e is T t)
                    {
                        component = t;
                        return true;
                    }
                }
            }
            component = null;
            return false;
        }
        public bool try_get_component(System.Type type, string name, out ProjectileComponent component)
        {
            if (m_component_name_indices.TryGetValue(name, out var list))
            {
                foreach (var e in list)
                {
                    if (type.IsAssignableFrom(e.GetType()))
                    {
                        component = e;
                        return true;
                    }
                }
            }
            component = null;
            return false;
        }
        public bool try_get_component(DeviceNode node, out ProjectileComponent component)
        {
            return m_component_node_indices.TryGetValue(node, out component);
        }
        public bool try_get_component<T>(DeviceNode node, out T component) where T : ProjectileComponent
        {
            if (m_component_node_indices.TryGetValue(node, out var e) && e is T t)
            {
                component = t;
                return true;
            }
            component = null;
            return false;
        }

        public Projectile(Vector2 p, Vector2 v, Vector2 d, Enum.EN_faction f)
        {
            position = p;
            velocity = v;
            direction = d;
            faction = f;
        }
        public void init(DeviceContext ctx,int projectile_id)
        {
            m_context = ctx;
            DB.instance.projectile.try_get((uint)projectile_id, out desc);

            current_hp = desc.f_projectile_hp;
        }
        public void tick()
        {
            foreach (var component in m_components_with_tick)
            {
                component.tick(m_context, this);
            }
        }
        public void destroy()
        {
            Mission.instance.try_get_mgr(Common.Config.ProjectileMgr_Name, out ProjectileMgr pmgr);
            pmgr.DestroyProjectile(this);
        }

        public void projectile_hurt(int dmg)
        {
            current_hp -= dmg;
            if (current_hp <= 0)
            {
                destroy();
            }
        }
    }
}

using CaravanEnhanced;
using DeviceGraph;
using System.Collections.Generic;


namespace Devices {

    public class Device {
        public UnityEngine.Vector2 velocity;
        public UnityEngine.Vector2 position;
        public UnityEngine.Vector2 direction;
        public int default_damage = 0;
        public Faction faction;
        public Item item;

        //==================================================================================================

        public DeviceContext ctx => m_context;
        private DeviceContext m_context;

        private List<DeviceComponent> m_components = new List<DeviceComponent>();
        private List<DeviceComponent> m_components_with_tick = new List<DeviceComponent>();
        private Dictionary<string, List<DeviceComponent>> m_component_name_indices = new Dictionary<string, List<DeviceComponent>>();
        private Dictionary<DeviceNode, DeviceComponent> m_component_node_indices = new Dictionary<DeviceNode, DeviceComponent>();

        public List<Projectile> projectiles = new List<Projectile>();

        //==================================================================================================


        public enum Faction {
            enemy,
            player,
            neutral,
        }


        public Device() {
            m_context = new DeviceContext(this);
        }

        public void init(DeviceGraphAsset asset, DeviceConfig[] config) {
            foreach (var node in asset.graph.nodes) {
                if (node is DeviceNode t) {
                    t.init(m_context, config);
                }
            }

            m_components_with_tick.Sort((a, b) => a.tick_order.CompareTo(b.tick_order));
        }

        public void start() {
            foreach (var component in m_components) {
                component.start(m_context);
            }
            foreach (var component in m_components_with_tick) {
                component.start(m_context);
            }
        }

        public void add_component(DeviceComponent component, bool need_tick) {

            if (need_tick) {
                m_components_with_tick.Add(component);
            } else {
                m_components.Add(component);
            }

            var name = component.name;
            if (name != null) {
                if (!m_component_name_indices.TryGetValue(name, out var list)) {
                    list = new List<DeviceComponent>();
                    m_component_name_indices.Add(name, list);
                }
                list.Add(component);
            }

            var node = component.graph_node;
            if (node != null) {
                m_component_node_indices.Add(node, component);
            }
        }

        public List<DeviceComponent> get_components(string name) {
            name = name ?? string.Empty;
            if (m_component_name_indices.TryGetValue(name ?? string.Empty, out var list)) {
                return list;
            }
            return null;
        }

        public bool try_get_component<T>(string name, out T component) where T : DeviceComponent {
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

        public bool try_get_component(System.Type type, string name, out DeviceComponent component) {
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

        public bool try_get_component(DeviceNode node, out DeviceComponent component) {
            return m_component_node_indices.TryGetValue(node, out component);
        }

        public bool try_get_provider<T>(string name, out T provider) where T : class, DeviceViews.IProvider {
            if (m_component_name_indices.TryGetValue(name, out var list)) {
                foreach (var e in list) {
                    if (e is T t) {
                        provider = t;
                        return true;
                    }
                }
            }
            provider = null;
            return false;
        }

        public bool try_get_component<T>(DeviceNode node, out T component) where T : DeviceComponent {
            if (m_component_node_indices.TryGetValue(node, out var e) && e is T t) {
                component = t;
                return true;
            }
            component = null;
            return false;
        }


        public void tick() {
            foreach (var component in m_components_with_tick) {
                component.tick(m_context);
            }
        }
    }
}

using World_Formal.BattleSystem.DeviceGraph;
using UnityEngine;
using Common_Formal;
using System.Collections.Generic;

namespace World_Formal.BattleSystem.Device{


    public class GraphDevice {

        public Caravans.Devices.Device device;

        public DeviceContext context => m_context;
        private DeviceContext m_context;

        private List<DeviceComponent> m_components = new List<DeviceComponent>();
        private List<DeviceComponent> m_components_with_tick = new List<DeviceComponent>();
        private Dictionary<string,List<DeviceComponent>> m_component_name_indices = new Dictionary<string, List<DeviceComponent>>();
        private Dictionary<DeviceNode,DeviceComponent>  m_component_node_indices = new Dictionary<DeviceNode,DeviceComponent>();

        //==========================================================================================

        public GraphDevice()
        {
            m_context = new DeviceContext(this);
        }


        public void init(DeviceGraphAsset asset)
        {
            foreach(var node in asset.graph.nodes)
            {
                if(node is DeviceNode d)
                {
                    d.init(m_context);
                }
            }
        }
        
        public void start()
        {
            foreach (var component in m_components)
            {
                component.start(m_context);
            }
            foreach (var component in m_components_with_tick)
            {
                component.start(m_context);
            }
        }

        public void tick()
        {
            foreach (var component in m_components_with_tick)
            {
                component.tick(m_context);
            }
        }

        #region component operations
        public void add_component(DeviceComponent component, bool need_tick)
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
                    list = new List<DeviceComponent>();
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

        public List<DeviceComponent> get_components(string name)
        {
            name = name ?? string.Empty;
            if (m_component_name_indices.TryGetValue(name ?? string.Empty, out var list))
            {
                return list;
            }
            return null;
        }

        public bool try_get_component<T>(string name, out T component) where T : DeviceComponent
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

        public bool try_get_component(System.Type type, string name, out DeviceComponent component)
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

        public bool try_get_component(DeviceNode node, out DeviceComponent component)
        {
            return m_component_node_indices.TryGetValue(node, out component);
        }

        public bool try_get_component<T>(DeviceNode node, out T component) where T : DeviceComponent
        {
            if (m_component_node_indices.TryGetValue(node, out var e) && e is T t)
            {
                component = t;
                return true;
            }
            component = null;
            return false;
        }

        //这一块暂时缺一个provider
        #endregion
    }
}

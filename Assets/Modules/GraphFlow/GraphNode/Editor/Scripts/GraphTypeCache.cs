
using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace GraphNode.Editor {

    public static class GraphTypeCache {


        public static GraphEditor create_graph_editor<T>() where T : GraphEditor {
            return create_graph_editor(typeof(T));
        }

        public static GraphEditor create_graph_editor(Type graph_type) {
            if (s_graph_types == null) {
                build_cache();
            }

            if (!s_graph_types.TryGetValue(graph_type, out var info)) {
                return null;
            }

            if (info.type == null) {
                var t = graph_type.BaseType;
                while (t != null) {
                    if (s_graph_types.TryGetValue(t, out var base_info)) {
                        if (base_info.type != null) {
                            info.type = base_info.type;
                            info.ctor = base_info.ctor;
                            break;
                        }
                    }
                    t = t.BaseType;
                }
            }
            if (info.ctor == null) {
                return null;
            }
            return info.ctor.Invoke(null) as GraphEditor;
        }

        public static NodeEditor create_node_editor<T>() where T : NodeEditor {
            return create_node_editor(typeof(T));
        }

        public static NodeEditor create_node_editor(Type node_type) {
            if (s_node_types == null) {
                build_cache();
            }

            if (!s_node_types.TryGetValue(node_type, out var info)) {
                return null;
            }

             if (info.type == null) {
                var t = node_type.BaseType;
                while (t != null) {
                    if (s_node_types.TryGetValue(t, out var base_info)) {
                        if (base_info.type != null) {
                            info.type = base_info.type;
                            info.ctor = base_info.ctor;
                            break;
                        }
                    }
                    t = t.BaseType;
                }
            }
            if (info.ctor == null) {
                return null;
            }
            return info.ctor.Invoke(null) as NodeEditor;
        }

        public static IEnumerator<Type> enumerate_graph_node_types<T>() where T : Graph {
            return enumerate_graph_node_types(typeof(T));
        }

        public static IEnumerator<Type> enumerate_graph_node_types(Type graph_type) {
            if (s_graph_types == null) {
                build_cache();
            }
            while (graph_type != null && s_graph_types.TryGetValue(graph_type, out var info)) {
                foreach (var t in info.node_types) {
                    yield return t;
                }
                graph_type = graph_type.BaseType;
            }
        }

        public static IEnumerator<NodePort> enumerate_node_static_ports<T>() where T : Node {
            return enumerate_node_static_ports(typeof(T));
        }

        public static IEnumerator<NodePort> enumerate_node_static_ports(Type node_type) {
            if (s_node_types == null) {
                build_cache();
            }

            var base_type = node_type.BaseType;
            if (base_type != null) {
                var iter = enumerate_node_static_ports(base_type);
                while (iter.MoveNext()) {
                    yield return iter.Current;
                }
            }

            if (s_node_types.TryGetValue(node_type, out var info)) {
                if (info.ports == null) {
                    info.ports = new List<NodePort>();
                    foreach (var node_attr in node_type.GetCustomAttributes()) {
                        if (node_attr is InputAttribute input) {
                            info.ports.Add(new NodeNodePort(NodePort.IO.Input, node_type, input.can_multi_connect));
                        } else if (node_attr is OutputAttribute output) {
                            info.ports.Add(new NodeNodePort(NodePort.IO.Output, node_type, output.can_multi_connect));
                        }
                    }
                    var flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
                    foreach (var pi in node_type.GetProperties(flags)) {
                        foreach (var attr in pi.GetCustomAttributes(false)) {
                            if (attr is InputAttribute input) {
                                var port = create_node_property_port(NodePort.IO.Input, pi, input.can_multi_connect);
                                if (port != null) {
                                    info.ports.Add(port);
                                } else {
                                    Debug.LogError($"GraphNode \'{node_type.Name}\': invalid Input \'{pi.Name}\'");
                                }
                            } else if (attr is OutputAttribute output) {
                                var port = create_node_property_port(NodePort.IO.Output, pi, output.can_multi_connect);
                                if (port != null) {
                                    info.ports.Add(port);
                                } else {
                                    Debug.LogError($"GraphNode \'{node_type.Name}\': invalid Output \'{pi.Name}\'");
                                }
                            }
                        }
                    }
                    foreach (var mi in node_type.GetMethods(flags)) {
                        foreach (var attr in mi.GetCustomAttributes(false)) {
                            if (attr is InputAttribute input) {
                                info.ports.Add(new NodeMethodPort(NodePort.IO.Input, mi, input.can_multi_connect));         
                            } else if (attr is OutputAttribute output) {
                                info.ports.Add(new NodeMethodPort(NodePort.IO.Output, mi, output.can_multi_connect));
                            }
                        }
                    }
                }
                foreach (var p in info.ports) {
                    yield return p;
                }
            }
        }
        
        public static PropertyEditor create_property_editor(Type property_type) {
            if (s_property_types == null) {
                build_cache();
            }

            while (property_type != null) {
                if (s_property_types.TryGetValue(property_type, out var info) && info.ctor != null) {
                    return (PropertyEditor)info.ctor.Invoke(null);
                }
                if (property_type.IsGenericType) {
                    var generic_type_def = property_type.GetGenericTypeDefinition();
                    if (s_property_types.TryGetValue(generic_type_def, out info)) {
                        var ty = info.type.MakeGenericType(property_type.GetGenericArguments());
                        var ctor = ty.GetConstructor(Type.EmptyTypes);
                        if (ctor != null) {
                            return (PropertyEditor)ctor.Invoke(null);
                        }
                    }
                    if (generic_type_def == typeof(List<>)) {
                        var element_type = property_type.GetGenericArguments();
                        element_type[0] = element_type[0].BaseType;
                        while (element_type[0] != null) {
                            if (s_property_types.TryGetValue(generic_type_def.MakeGenericType(element_type), out info) && info.ctor != null) {
                                return (PropertyEditor)info.ctor.Invoke(null);
                            }
                            element_type[0] = element_type[0].BaseType;
                        }
                    }
                }
                property_type = property_type.BaseType;
            }

            return null;
        }

        public static IEnumerator<(FieldInfo, PropertyEditor)> enumerate_properties(Type type) {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (var fi in type.GetFields(flags)) {
                if (fi.GetCustomAttribute<NonPropertyAttribute>() != null) {
                    continue;
                }
                var editor = create_property_editor(fi.FieldType);
                if (editor != null) {
                    yield return (fi, editor);
                }
            }
        }

        public static NodePort create_node_property_port(NodePort.IO io, PropertyInfo pi, bool multi) {
            if (s_port_types == null) {
                build_cache();
            }
            var type = pi.PropertyType;
            ConstructorInfo ctor;
            for (;;) {
                if (s_port_types.TryGetValue(type, out ctor)) {
                    return (NodePort)ctor.Invoke(new object[] { io, pi, multi });
                }
                if (type.IsGenericType) {
                    var gd = type.GetGenericTypeDefinition();
                    if (s_port_types.TryGetValue(gd, out ctor)) {
                        return (NodePort)ctor.Invoke(new object[] { io, pi, multi });
                    }
                    if (gd == typeof(List<>)) {
                        return create_node_property_list_port(io, pi, multi, type.GetGenericArguments()[0]);
                    }
                }
                type = type.BaseType;
                if (type == null) {
                    return null;
                }
            }
        }

        private static NodePort create_node_property_list_port(NodePort.IO io, PropertyInfo pi, bool multi, Type element_type) {
            var list_type = typeof(List<>);
            for (;;) {
                if (s_port_types.TryGetValue(list_type.MakeGenericType(element_type), out var ctor)) {
                    return (NodePort)ctor.Invoke(new object[] { io, pi, multi });
                }
                element_type = element_type.BaseType;
                if (element_type == null) {
                    return null;
                }
            }
        }

        private static void build_cache() {
            s_graph_types = new Dictionary<Type, GraphEditorInfo>();
            s_node_types = new Dictionary<Type, NodeEditorInfo>();
            s_property_types = new Dictionary<Type, EditorInfo>();
            s_port_types = new Dictionary<Type, ConstructorInfo>();

            var graph_type = typeof(Graph);
            var graph_editor_type = typeof(GraphEditor);
            var node_type = typeof(Node);
            var node_editor_type = typeof(NodeEditor);
            var property_editor_type = typeof(PropertyEditor);
            var node_port_type = typeof(NodePort);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var name = assembly.GetName().Name;
                var idx = name.IndexOf('.');
                if (idx != -1) {
                    name = name.Substring(0, idx);
                }
                switch (name) {
                    case "Foundation":
                    case "UnityEditor":
                    case "UnityEngine":
                    case "System":
                    case "mscorlib":
                        continue;
                    default:
                        foreach (var type in assembly.GetTypes()) {
                            if (type.IsAbstract) {
                                continue;
                            }
                            if (graph_editor_type.IsAssignableFrom(type)) {
                                foreach (GraphEditorAttribute attr in type.GetCustomAttributes(typeof(GraphEditorAttribute))) {
                                    if (!graph_type.IsAssignableFrom(attr.graph_type)) {
                                        Debug.LogError($"GraphEditor \'{type.Name}\': invalid graph type \'{attr.graph_type.Name}\'");
                                        continue;
                                    }
                                    if (s_graph_types.TryGetValue(attr.graph_type, out var info)) {
                                        if (info.type != null) {
                                            Debug.LogError($"GraphEditor \'{type.Name}\': duplicated");
                                            continue;
                                        }
                                    } else {
                                        info = new GraphEditorInfo();
                                        s_graph_types.Add(attr.graph_type, info);
                                    }
                                    info.type = type;
                                    info.ctor = type.GetConstructor(Type.EmptyTypes);
                                }
                                continue;
                            }

                            if (graph_type.IsAssignableFrom(type)) {
                                if (!s_graph_types.ContainsKey(type)) {
                                    s_graph_types.Add(type, new GraphEditorInfo());
                                }
                                continue;
                            }

                            if (node_editor_type.IsAssignableFrom(type)) {
                                foreach (NodeEditorAttribute attr in type.GetCustomAttributes(typeof(NodeEditorAttribute))) {
                                    if (!node_type.IsAssignableFrom(attr.node_type)) {
                                        Debug.LogError($"NodeEditor \'{type.Name}\': invalid node type \'{attr.node_type}\'");
                                        continue;
                                    }
                                    
                                    if (s_node_types.TryGetValue(attr.node_type, out var info)) {
                                        if (info.type != null) {
                                            Debug.LogError($"NodeEditor \'{type.Name}\': duplicated");
                                            continue;
                                        }
                                    } else {
                                        info = new NodeEditorInfo();
                                        s_node_types.Add(attr.node_type, info);
                                    }
                                    info.type = type;
                                    info.ctor = type.GetConstructor(Type.EmptyTypes);
                                }
                                continue;
                            }

                            if (node_type.IsAssignableFrom(type)) {
                                if (!type.IsGenericType) {
                                    var nt = type;
                                l_node_type:
                                    if (!s_node_types.ContainsKey(nt)) {
                                        s_node_types.Add(nt, new NodeEditorInfo());
                                    }
                                    foreach (GraphAttribute attr in nt.GetCustomAttributes(typeof(GraphAttribute))) {
                                        foreach (var gt in attr.graphs) {
                                            if (!graph_type.IsAssignableFrom(gt)) {
                                                Debug.LogError($"Node \'{type.Name}\': invalid graph type \'{gt.Name}\'");
                                                continue;
                                            }
                                            if (!s_graph_types.TryGetValue(gt, out var ge)) {
                                                ge = new GraphEditorInfo();
                                                s_graph_types.Add(gt, ge);
                                            }
                                            ge.node_types.Add(nt);
                                        }
                                    }
                                    nt = nt.BaseType;
                                    if (nt.IsGenericType) {
                                        goto l_node_type;
                                    }
                                }
                                continue;
                            }

                            if (property_editor_type.IsAssignableFrom(type)) {
                                foreach (PropertyEditorAttribute attr in type.GetCustomAttributes(typeof(PropertyEditorAttribute))) {
                                    try {
                                        var info = new EditorInfo();
                                        info.type = type;
                                        info.ctor = type.GetConstructor(Type.EmptyTypes);
                                        s_property_types.Add(attr.property_type, info);
                                    } catch (ArgumentException) {
                                        Debug.LogError($"PropertyEditor \'{type.Name}\': duplicated");
                                    }
                                }
                            }

                            if (node_port_type.IsAssignableFrom(type)) {
                                foreach (NodePropertyPortAttribute attr in type.GetCustomAttributes(typeof(NodePropertyPortAttribute))) {
                                    try {
                                        var ctor = type.GetConstructor(s_port_constructor_param_types);
                                        if (ctor != null) {
                                            s_port_types.Add(attr.property_type, ctor);
                                        } else {
                                            Debug.LogError($"PropertyEditor \'{type.Name}\': no required ctor");
                                        }
                                    } catch (ArgumentException) {
                                        Debug.LogError($"NodePropertyPort \'{type.Name}\': duplicated");
                                    }
                                }
                                
                            }
                        }
                        break;
                }
            }

        }
        
        private class EditorInfo {
            public Type type = null;
            public ConstructorInfo ctor = null;
        }

        private class GraphEditorInfo : EditorInfo {
            public HashSet<Type> node_types = new HashSet<Type>();
        }

        private class NodeEditorInfo : EditorInfo {
            public List<NodePort> ports = null;
        }

        private static Dictionary<Type, GraphEditorInfo> s_graph_types = null;
        private static Dictionary<Type, NodeEditorInfo> s_node_types = null;
        private static Dictionary<Type, EditorInfo> s_property_types = null;
        private static Dictionary<Type, ConstructorInfo> s_port_types = null;

        private static readonly Type[] s_port_constructor_param_types = new Type[] { typeof(NodePort.IO), typeof(PropertyInfo), typeof(bool) };
    }

}
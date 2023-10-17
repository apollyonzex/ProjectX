using System.Collections.Generic;
using System.Reflection;
using CalcExpr;

namespace GraphNode {

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class GraphActionAttribute : System.Attribute {
        public string name { get; }
        public GraphActionAttribute() { }
        public GraphActionAttribute(string name) { this.name = name; }
    }

    public enum ActionParameterType {
        Content = 0,
        Integer = 1,
        Floating = 2,
        Boolean = 3,
    }

    [System.Serializable]
    public class ActionMethod {

        public ActionParameterType[] parameter_types;
        public string method_name;

        public MethodInfo method_info => m_method_info;

        [System.NonSerialized]
        private MethodInfo m_method_info;

        [System.NonSerialized]
        private System.Type m_initialized;

        public void initialize(System.Type obj_type) {
            if (m_initialized != obj_type) {
                m_initialized = obj_type;
                if (obj_type == null || string.IsNullOrEmpty(method_name) || parameter_types == null) {
                    m_method_info = null;
                    return;
                }
                System.Type[] ps;
                if (parameter_types.Length == 0) {
                    ps = System.Type.EmptyTypes;
                } else {
                    ps = new System.Type[parameter_types.Length];
                    for (int i = 0; i < parameter_types.Length; ++i) {
                        ps[i] = Utility.get_type((ValueType)parameter_types[i]) ?? typeof(string);
                    }
                }
                m_method_info = obj_type.GetMethod(method_name, ps);
                if (m_method_info == null && obj_type.IsInterface) {
                    get_interface_method_info(obj_type, ps);
                }
            }
        }

        bool get_interface_method_info(System.Type ty, System.Type[] ps) {
            foreach (var t in ty.GetInterfaces()) {
                m_method_info = t.GetMethod(method_name, ps);
                if (m_method_info != null || get_interface_method_info(t, ps)) {
                    return true;
                }
            }
            return false;
        }

        public bool invoke(System.Type obj_type, object obj, IExpression[] parameters, out object ret) {
            initialize(obj_type);
            if (m_method_info == null) {
                ret = null;
                return false;
            }
            if (parameters.Length != parameter_types.Length) {
                ret = null;
                return false;
            }
            object[] ps;
            if (parameters.Length == 0) {
                ps = null;
            } else {
                ps = new object[parameters.Length];
                for (int i = 0; i < ps.Length; ++i) {
                    var pt = parameter_types[i];
                    if (pt == ActionParameterType.Content) {
                        ps[i] = parameters[i].content;
                    } else if (parameters[i].calc(obj, obj_type, out int val)) {
                        ps[i] = Utility.convert_from((ValueType)pt, (uint)val);
                    } else {
                        ret = null;
                        return false;
                    }
                }
            }
            
            ret = m_method_info.Invoke(obj, ps);
            return true;
        }

        public static IEnumerable<(string, ActionMethod)> collect_methods(System.Type type) {
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                var ps = mi.GetParameters();
                var pts = new ActionParameterType[ps.Length];
                var ok = true;
                for (int i = 0; i < ps.Length; ++i) {
                    var pt = ps[i].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t)) {
                        yield return e;
                    }
                }
            }
        }

        public static IEnumerable<(string, ActionMethod)> collect_methods(System.Type type, System.Type ret_type) {
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                if (!ret_type.IsAssignableFrom(mi.ReturnType)) {
                    continue;
                }
                var ps = mi.GetParameters();
                var pts = new ActionParameterType[ps.Length];
                var ok = true;
                for (int i = 0; i < ps.Length; ++i) {
                    var pt = ps[i].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t, ret_type)) {
                        yield return e;
                    }
                }
            }
        }

        public static IEnumerable<(string, ActionMethod)> collect_methods(System.Type type, TypeFilter ret_type_filter) {
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                if (!ret_type_filter.check(mi.ReturnType)) {
                    continue;
                }
                var ps = mi.GetParameters();
                var pts = new ActionParameterType[ps.Length];
                var ok = true;
                for (int i = 0; i < ps.Length; ++i) {
                    var pt = ps[i].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t, ret_type_filter)) {
                        yield return e;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ActionMethod<T> {

        public ActionParameterType[] parameter_types;
        public string method_name;

        public MethodInfo method_info => m_method_info;

        [System.NonSerialized]
        private MethodInfo m_method_info;

        [System.NonSerialized]
        private System.Type m_initialized;

        public void initialize(System.Type obj_type) {
            if (m_initialized != obj_type) {
                m_initialized = obj_type;
                if (obj_type == null || string.IsNullOrEmpty(method_name) || parameter_types == null) {
                    m_method_info = null;
                    return;
                }
                System.Type[] ps;
                if (parameter_types.Length == 0) {
                    ps = new System.Type[] { typeof(T) };
                } else {
                    ps = new System.Type[parameter_types.Length + 1];
                    ps[0] = typeof(T);
                    for (int i = 0; i < parameter_types.Length; ++i) {
                        ps[i + 1] = Utility.get_type((ValueType)parameter_types[i]) ?? typeof(string);
                    }
                }
                m_method_info = obj_type.GetMethod(method_name, ps);
                if (m_method_info == null && obj_type.IsInterface) {
                    get_interface_method_info(obj_type, ps);
                }
            }
        }

        bool get_interface_method_info(System.Type ty, System.Type[] ps) {
            foreach (var t in ty.GetInterfaces()) {
                m_method_info = t.GetMethod(method_name, ps);
                if (m_method_info != null || get_interface_method_info(t, ps)) {
                    return true;
                }
            }
            return false;
        }

        public bool invoke(System.Type obj_type, object obj, T param, IExpression[] parameters, out object ret) {
            initialize(obj_type);
            if (m_method_info == null) {
                ret = null;
                return false;
            }
            if (parameters.Length != parameter_types.Length) {
                ret = null;
                return false;
            }
            var ps = new object[parameters.Length + 1];
            ps[0] = param;
            for (int i = 0; i < parameters.Length; ++i) {
                var pt = parameter_types[i];
                if (pt == ActionParameterType.Content) {
                    ps[i + 1] = parameters[i].content;
                } else if (parameters[i].calc(obj, obj_type, out int val)) {
                    ps[i + 1] = Utility.convert_from((ValueType)pt, (uint)val);
                } else {
                    ret = null;
                    return false;
                }
            }
            ret = m_method_info.Invoke(obj, ps);
            return true;
        }

        public static IEnumerable<(string, ActionMethod<T>)> collect_methods(System.Type type) {
            var param_type = typeof(T);
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                var ps = mi.GetParameters();
                if (ps.Length == 0 || ps[0].ParameterType != param_type) {
                    continue;
                }
                var pts = new ActionParameterType[ps.Length - 1];
                var ok = true;
                for (int i = 0; i < pts.Length; ++i) {
                    var pt = ps[i + 1].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod<T> {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t)) {
                        yield return e;
                    }
                }
            }
        }

        public static IEnumerable<(string, ActionMethod<T>)> collect_methods(System.Type type, System.Type ret_type) {
            var param_type = typeof(T);
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                var ps = mi.GetParameters();
                if (ps.Length == 0 || ps[0].ParameterType != param_type) {
                    continue;
                }
                if (!ret_type.IsAssignableFrom(mi.ReturnType)) {
                    continue;
                }
                var pts = new ActionParameterType[ps.Length - 1];
                var ok = true;
                for (int i = 0; i < pts.Length; ++i) {
                    var pt = ps[i + 1].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod<T> {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t, ret_type)) {
                        yield return e;
                    }
                }
            }
        }

        public static IEnumerable<(string, ActionMethod<T>)> collect_methods(System.Type type, TypeFilter ret_type_filter) {
            var param_type = typeof(T);
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null) {
                    continue;
                }
                var ps = mi.GetParameters();
                if (ps.Length == 0 || ps[0].ParameterType != param_type) {
                    continue;
                }
                if (!ret_type_filter.check(mi.ReturnType)) {
                    continue;
                }
                var pts = new ActionParameterType[ps.Length - 1];
                var ok = true;
                for (int i = 0; i < pts.Length; ++i) {
                    var pt = ps[i + 1].ParameterType;
                    if (pt == typeof(string)) {
                        pts[i] = ActionParameterType.Content;
                    } else if (pt == typeof(int)) {
                        pts[i] = ActionParameterType.Integer;
                    } else if (pt == typeof(float)) {
                        pts[i] = ActionParameterType.Floating;
                    } else if (pt == typeof(bool)) {
                        pts[i] = ActionParameterType.Boolean;
                    } else {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return (attr.name ?? mi.Name, new ActionMethod<T> {
                        parameter_types = pts,
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = type,
                    });
                }
            }
            if (type.IsInterface) {
                foreach (var t in type.GetInterfaces()) {
                    foreach (var e in collect_methods(t, ret_type_filter)) {
                        yield return e;
                    }
                }
            }
        }
    }
}
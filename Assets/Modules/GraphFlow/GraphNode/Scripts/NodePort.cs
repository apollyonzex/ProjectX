
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;

namespace GraphNode {

    [Serializable]
    public abstract class NodePort {
        public enum IO {
            Unknown = 0,
            Input = 1,
            Output = 2,
        }

        
        public abstract IO io { get; }

        public abstract string name { get; }
        public abstract MethodInfo method_info { get; }
        public abstract bool is_delegate { get; }
        public abstract bool is_static { get; }
        public abstract bool can_mulit_connect { get; }
        public abstract bool can_connect_with(Node node, Node other, NodePort port);
        public abstract void connect_unchecked(Node node, Node other, NodePort port);
        public abstract void disconnect_unchecked(Node node, Node other, NodePort port);

        protected void set_sign(Type ret, Type[] parameters) {
            m_ret = ret;
            m_parameters = parameters;
            m_hash_code = ret.GetHashCode();
            foreach (var t in parameters) {
                m_hash_code ^= t.GetHashCode();
            }
        }

        protected bool check_sign(NodePort other) {
            if (m_hash_code == other.m_hash_code && m_ret == other.m_ret && m_parameters.Length == other.m_parameters.Length) {
                for (int i = 0; i < m_parameters.Length; ++i) {
                    if (m_parameters[i] != other.m_parameters[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public Type sign_ret => m_ret;
        public IReadOnlyList<Type> sign_params => m_parameters;
        public int sign_hash_code => m_hash_code;

        [NonSerialized]
        private Type m_ret;

        [NonSerialized]
        private Type[] m_parameters;

        [NonSerialized]
        private int m_hash_code;
    }

    [Serializable]
    public sealed class NodeNodePort : NodePort, IDeserializationCallback {

        public NodeNodePort(IO io, Type type, bool multi) {
            m_io = io;
            m_type = type;
            m_multi = multi;
            build_sign_and_name();
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            build_sign_and_name();
        }

        public override bool is_static => true;
        public override string name => m_name;
        public override IO io => m_io;
        public override bool can_mulit_connect => m_multi;
        public override bool is_delegate => false;
        public override MethodInfo method_info => null;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (port is NodeNodePort) {
                return false;
            }
            return port.can_connect_with(other, node, this);
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            port.connect_unchecked(other, node, this);
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            port.disconnect_unchecked(other, node, this);
        }

        public override int GetHashCode() {
            return m_type.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is NodeNodePort other && m_type == other.m_type;
        }

        private void build_sign_and_name() {
            set_sign(m_type, Type.EmptyTypes);
            var attr = m_type.GetCustomAttribute<DisplayAttribute>();
            if (attr != null) {
                m_name = attr.name;
            }
        }

        private Type m_type;

        [NonSerialized]
        private IO m_io = IO.Unknown;

        [NonSerialized]
        private bool m_multi;

        [NonSerialized]
        private string m_name = string.Empty;
    }

    [Serializable]
    public abstract class NodePropertyPort : NodePort, IDeserializationCallback {
        public NodePropertyPort(IO io, PropertyInfo pi, bool multi) {
            m_io = io;
            m_pi = pi;
            m_multi = multi;
            build_name();
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            on_deserialization();
        }

        protected virtual void on_deserialization() {
            build_name();
        }

        public sealed override bool is_static => true;
        public override string name => m_name;
        public override IO io => m_io;
        public override bool can_mulit_connect => m_multi;

        public override int GetHashCode() {
            return m_pi.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is NodePropertyPort other && m_pi == other.m_pi;
        }

        protected PropertyInfo m_pi;

        [NonSerialized]
        protected IO m_io = IO.Unknown;

        private void build_name() {
            var attr = m_pi.GetCustomAttribute<DisplayAttribute>();
            if (attr != null) {
                m_name = attr.name;
            } else {
                m_name = m_pi.Name;
            }
        }

        [NonSerialized]
        protected string m_name = null;

        [NonSerialized]
        private bool m_multi;
    }

    [Serializable][NodePropertyPort(typeof(Node))]
    public class NodePropertyPort_Node : NodePropertyPort {
        public NodePropertyPort_Node(IO io, PropertyInfo pi, bool _) : base(io, pi, false) {
            set_sign(pi.PropertyType, Type.EmptyTypes);
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            set_sign(m_pi.PropertyType, Type.EmptyTypes);
        }

        public sealed override MethodInfo method_info => null;
        public sealed override bool is_delegate => false;
        public sealed override bool can_mulit_connect => false;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port is NodeNodePort np) {
                if (m_pi.PropertyType.IsAssignableFrom(np.sign_ret)) {
                    return true;
                }
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            m_pi.SetValue(node, other);
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            m_pi.SetValue(node, null);
        }
    }

    [Serializable][NodePropertyPort(typeof(List<Node>))]
    public class NodePropertyPort_NodeList : NodePropertyPort {
        public NodePropertyPort_NodeList(IO io, PropertyInfo pi, bool multi) : base(io, pi, multi) {
            set_sign(pi.PropertyType, Type.EmptyTypes);
            build_element_type();
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            set_sign(m_pi.PropertyType, Type.EmptyTypes);
        }

        public sealed override MethodInfo method_info => null;
        public sealed override bool is_delegate => false;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port is NodeNodePort np) {
                if (m_element_type.IsAssignableFrom(np.sign_ret)) {
                    if (can_mulit_connect && node != null && other != null) {
                        return !(get_value(node) as IList).Contains(other);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var value = get_value(node) as IList;
            if (value != null) {
                if (!can_mulit_connect) {
                    value.Clear();
                }
                value.Add(other);
            }
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            var value = get_value(node) as IList;
            if (value != null) {
                if (can_mulit_connect) {
                    value.Remove(other);
                } else {
                    value.Clear();
                }
            }
        }

        protected object get_value(object obj) {
            var ret = m_pi.GetValue(obj);
            if (ret == null) {
                try {
                    ret = Activator.CreateInstance(m_pi.PropertyType);
                    m_pi.SetValue(obj, ret);
                } catch (Exception) {

                }
            }
            return ret;
        }

        [NonSerialized]
        Type m_element_type;

        void build_element_type() {
            var ty = m_pi.PropertyType;
            do {
                if (ty.IsGenericType) {
                    if (ty.GetGenericTypeDefinition() == typeof(List<>)) {
                        m_element_type = ty.GetGenericArguments()[0];
                        break;
                    }
                }
                ty = ty.BaseType;
            } while (ty != null);
        }

        public Type element_type => m_element_type;
    }


    [Serializable][NodePropertyPort(typeof(Delegate))]
    public class NodePropertyPort_Delegate : NodePropertyPort {

        public NodePropertyPort_Delegate(IO io, PropertyInfo pi, bool multi) : base(io, pi, multi) {
            build_sign();
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            build_sign();
        }

        public override MethodInfo method_info => null;
        public override bool is_delegate => true;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port.method_info != null) {
                if (check_sign(port)) {
                    if (can_mulit_connect && node != null && other != null) {
                        var value = m_pi.GetValue(node) as Delegate;
                        if (value != null) {
                            var list = value.GetInvocationList();
                            foreach (var item in list) {
                                if (item.Target == other && item.Method == port.method_info) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var t = Delegate.CreateDelegate(m_pi.PropertyType, other, port.method_info);
            if (can_mulit_connect) {
                var value = m_pi.GetValue(node) as Delegate;
                if (value != null) {
                    m_pi.SetValue(node, Delegate.Combine(value, t));
                } else {
                    m_pi.SetValue(node, t);
                }                
            } else {
                m_pi.SetValue(node, t);
            }
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            if (can_mulit_connect) {
                m_pi.SetValue(node, Delegate.Remove(m_pi.GetValue(node) as Delegate, Delegate.CreateDelegate(m_pi.PropertyType, other, port.method_info)));
            } else {
                m_pi.SetValue(node, null);
            }
        }

        private void build_sign() {
            var mi = m_pi.PropertyType.GetMethod("Invoke");
            var ps = mi.GetParameters();
            var parameters = new Type[ps.Length];
            for (int i = 0; i < ps.Length; ++i) {
                parameters[i] = ps[i].ParameterType;
            }
            set_sign(mi.ReturnType, parameters);
        }
    }
    
    [Serializable]
    public class NodeMethodPort : NodePort, IDeserializationCallback {

        public NodeMethodPort(IO io, MethodInfo mi, bool multi) {
            m_io = io;
            m_mi = mi;
            m_multi = multi;
            build_sign_and_name();
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            build_sign_and_name();
        }

        public override int GetHashCode() {
            return m_mi.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is NodeMethodPort other && m_mi == other.m_mi;
        }

        public override IO io => m_io;
        public override string name => m_name;
        public override MethodInfo method_info => m_mi;
        public override bool is_delegate => false;
        public sealed override bool is_static => true;
        public sealed override bool can_mulit_connect => m_multi;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port.is_delegate) {
                return port.can_connect_with(other, node, this);
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            port.connect_unchecked(other, node, this);
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            port.disconnect_unchecked(other, node, this);
        }

        private void build_sign_and_name() {
            var ps = m_mi.GetParameters();
            var parameters = new Type[ps.Length];
            for (int i = 0; i < ps.Length; ++i) {
                parameters[i] = ps[i].ParameterType;
            }
            set_sign(m_mi.ReturnType, parameters);
            var attr = m_mi.GetCustomAttribute<DisplayAttribute>();
            if (attr != null) {
                m_name = attr.name;
            } else {
                m_name = m_mi.Name;
            }
        }

        private MethodInfo m_mi;

        [NonSerialized]
        private string m_name = null;

        [NonSerialized]
        private IO m_io = IO.Unknown;

        [NonSerialized]
        private bool m_multi;
    }

    [Serializable]
    public abstract class NodeDynamicPort : NodePort {
        public sealed override bool is_static => false;
    }

    [Serializable]
    public abstract class NodeDynamicNodePort<T> : NodeDynamicPort, IDeserializationCallback where T : Node {
        public NodeDynamicNodePort() {
            set_sign(typeof(T), Type.EmptyTypes);
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            set_sign(typeof(T), Type.EmptyTypes);
        }

        public T value = null;

        public sealed override bool can_mulit_connect => false;
        public sealed override bool is_delegate => false;
        public sealed override MethodInfo method_info => null;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port is NodeNodePort np) {
                if (typeof(T).IsAssignableFrom(np.sign_ret)) {
                    return true;
                }
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            value = other as T;
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            value = null;
        }
    }

    [Serializable]
    public abstract class NodeDelegatePort<T> : NodeDynamicPort, IDeserializationCallback where T : Delegate {

        public NodeDelegatePort() {
            set_sign();
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            set_sign();
        }

        public T value = null;

        public sealed override bool is_delegate => true;
        public sealed override MethodInfo method_info => null;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port.method_info != null) {
                if (check_sign(port)) {
                    if (can_mulit_connect && other != null) {
                        if (value != null) {
                            var list = value.GetInvocationList();
                            foreach (var item in list) {
                                if (item.Target == other && item.Method == port.method_info) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
                
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var t = Delegate.CreateDelegate(typeof(T), other, port.method_info);
            if (can_mulit_connect) {
                if (value != null) {
                    value = (T)Delegate.Combine(value, t);
                } else {
                    value = (T)t;
                }
            } else {
                value = (T)t;
            }
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            if (can_mulit_connect) {
                value = (T)Delegate.Remove(value, Delegate.CreateDelegate(typeof(T), other, port.method_info));
            } else {
                value = null;
            }
        }

        private void set_sign() {
            var mi = typeof(T).GetMethod("Invoke");
            var ps = mi.GetParameters();
            var parameters = new Type[ps.Length];
            for (int i = 0; i < ps.Length; ++i) {
                parameters[i] = ps[i].ParameterType;
            }
            set_sign(mi.ReturnType, parameters);
        }
    }

    [Serializable]
    public abstract class NodeMethodInfoPort : NodeDynamicPort, IDeserializationCallback {

        public NodeMethodInfoPort(MethodInfo value) {
            m_value = value;
            build_sign_and_name();
        }

        void IDeserializationCallback.OnDeserialization(object sender) {
            build_sign_and_name();
        }

        protected virtual void build_sign_and_name() {
            var ps = m_value.GetParameters();
            var parameters = new Type[ps.Length];
            for (int i = 0; i < ps.Length; ++i) {
                parameters[i] = ps[i].ParameterType;
            }
            set_sign(m_value.ReturnType, parameters);
            var attr = m_value.GetCustomAttribute<DisplayAttribute>();
            if (attr != null) {
                m_name = attr.name;
            } else {
                m_name = m_value.Name;
            }
        }

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port.is_delegate) {
                return port.can_connect_with(other, node, this);
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            port.connect_unchecked(other, node, this);
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            port.disconnect_unchecked(other, node, this);
        }

        MethodInfo m_value;
        string m_name;

        public override string name => m_name;

        public override MethodInfo method_info => m_value;

        public override bool is_delegate => false;
    }

}
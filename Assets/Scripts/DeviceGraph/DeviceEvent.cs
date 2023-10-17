
using GraphNode;
using System.Reflection;
using System.Collections.Generic;

namespace DeviceGraph {

    [System.Serializable]
    public class DeviceEvent {

        public void invoke(DeviceContext ctx) {
            foreach (var action in common_actions) {
                action.Invoke(ctx);
            }
        }

        public virtual bool add_unchecked(System.Delegate obj) {
            if (obj is System.Action<DeviceContext> action) {
                common_actions.Add(action);
                return true;
            }
            return false;
        }

        public virtual bool contains(System.Delegate obj) {
            if (obj is System.Action<DeviceContext> action) {
                return common_actions.Contains(action);
            }
            return false;
        }

        public virtual bool remove(System.Delegate obj) {
            if (obj is System.Action<DeviceContext> action) {
                return common_actions.Remove(action);
            }
            return false;
        }

        public readonly List<System.Action<DeviceContext>> common_actions = new List<System.Action<DeviceContext>>();
    }


    [System.Serializable]
    public class DeviceEvent<T> : DeviceEvent {

        public void invoke(DeviceContext ctx, T obj) {
            foreach (var action in actions) {
                action.Invoke(ctx, obj);
            }
            invoke(ctx);
        }

        public override bool add_unchecked(System.Delegate obj) {
            if (obj is System.Action<DeviceContext, T> action) {
                actions.Add(action);
                return true;
            }
            return base.add_unchecked(obj);
        }

        public override bool contains(System.Delegate obj) {
            if (obj is System.Action<DeviceContext, T> action) {
                return actions.Contains(action);
            }
            return base.contains(obj);
        }

        public override bool remove(System.Delegate obj) {
            if (obj is System.Action<DeviceContext, T> action) {
                return actions.Remove(action);
            }
            return base.remove(obj);
        }

        public readonly List<System.Action<DeviceContext, T>> actions = new List<System.Action<DeviceContext, T>>(); 
    }

    [System.Serializable]
    [NodePropertyPort(typeof(DeviceEvent))]
    public class DeviceNodePort_Event : NodePropertyPort {

        public DeviceNodePort_Event(IO io, PropertyInfo pi, bool _) : base(io, pi, true) {
            set_sign(typeof(void), new System.Type[] { typeof(DeviceContext) });
        }

        public sealed override MethodInfo method_info => null;

        public sealed override bool is_delegate => true;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (!io.can_connect_with(port.io) || port.method_info == null) {
                return false;
            }
            if (check_sign(port)) {
                if (other != null) {
                    var value = (DeviceEvent)m_pi.GetValue(node);
                    if (value.contains(System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info))) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var value = (DeviceEvent)m_pi.GetValue(node);
            value.add_unchecked(System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info));
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            var value = (DeviceEvent)m_pi.GetValue(node);
            value.remove(System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info));
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            set_sign(typeof(void), new System.Type[] { typeof(DeviceContext) });
        }
    }

    [System.Serializable]
    [NodePropertyPort(typeof(DeviceEvent<>))]
    public class DeviceNodePort_EventWith : NodePropertyPort {

        public DeviceNodePort_EventWith(IO io, PropertyInfo pi, bool _) : base(io, pi, true) {
            build_sign();
        }

        public sealed override MethodInfo method_info => null;

        public sealed override bool is_delegate => true;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (!io.can_connect_with(port.io) || port.method_info == null) {
                return false;
            }
            if (check_sign(port)) {
                if (other != null) {
                    var value = (DeviceEvent)m_pi.GetValue(node);
                    if (value.contains(System.Delegate.CreateDelegate(m_action_type, other, port.method_info))) {
                        return false;
                    }
                }
                return true;
            }
            if (port.sign_ret == sign_ret) {
                var pi = port.sign_params;
                if (pi.Count == 1 && pi[0] == typeof(DeviceContext)) {
                    if (other != null) {
                        var value = (DeviceEvent)m_pi.GetValue(node);
                        if (value.contains(System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info))) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var value = (DeviceEvent)m_pi.GetValue(node);
            System.Delegate action;
            if (port.sign_params.Count == 1) {
                action = System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info);
            } else {
                action = System.Delegate.CreateDelegate(m_action_type, other, port.method_info);
            }
            value.add_unchecked(action);
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            var value = (DeviceEvent)m_pi.GetValue(node);
            System.Delegate action;
            if (port.sign_params.Count == 1) {
                action = System.Delegate.CreateDelegate(typeof(System.Action<DeviceContext>), other, port.method_info);
            } else {
                action = System.Delegate.CreateDelegate(m_action_type, other, port.method_info);
            }
            value.remove(action);
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            build_sign();
        }

        private void build_sign() {
            var pt = m_pi.PropertyType;
            var gt = typeof(DeviceEvent<>);
            do {
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == gt) {
                    var ps = new System.Type[] { typeof(DeviceContext), pt.GetGenericArguments()[0] };
                    set_sign(typeof(void), ps);
                    m_action_type = typeof(System.Action<,>).MakeGenericType(ps);
                    break;
                }
                pt = pt.BaseType;
            } while (pt != null);
        }

        private System.Type m_action_type;
    }
}

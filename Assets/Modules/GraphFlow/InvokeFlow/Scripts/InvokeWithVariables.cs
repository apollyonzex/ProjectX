
using System.Reflection;
using System.Collections.Generic;
using GraphNode;
using UnityEngine;

namespace InvokeFlow {

    [System.Serializable]
    public class InvokeWithVariables {

        public interface IPort {
            IEnumerator<Variable> enumerate_variables();
        }

        public InvokeWithVariables(InvokeNode.Invoke action) {
            this.action = action;
        }

        public InvokeNode.Invoke action { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class ParameterAttribute : System.Attribute {
        public readonly Parameter parameter;
        public ParameterAttribute(VariableType type, string name) {
            parameter = new Parameter(type, name);
        }
    }

    [System.Serializable][NodePropertyPort(typeof(InvokeWithVariables))]
    public class NodePropertyPort_InvokeWithVariables : NodePropertyPort, InvokeWithVariables.IPort {
        public NodePropertyPort_InvokeWithVariables(IO io, PropertyInfo pi, bool multi) : base(io, pi, multi) {
            set_sign(typeof(InvokeState), new System.Type[] { typeof(IContext) });
        }

        protected override void on_deserialization() {
            base.on_deserialization();
            set_sign(typeof(InvokeState), new System.Type[] { typeof(IContext) });
        }

        public override IO io => m_io;
        public override bool is_delegate => true;
        public override MethodInfo method_info => null;
        public sealed override bool can_mulit_connect => false;

        public override bool can_connect_with(Node node, Node other, NodePort port) {
            if (io.can_connect_with(port.io) && port.method_info != null) {
                return check_sign(port);
            }
            return false;
        }

        public override void connect_unchecked(Node node, Node other, NodePort port) {
            var ctor = m_pi.PropertyType.GetConstructor(new System.Type[] { typeof(InvokeNode.Invoke) });
            if (ctor != null) {
                m_pi.SetValue(node, ctor.Invoke(new object[] { System.Delegate.CreateDelegate(typeof(InvokeNode.Invoke), other, port.method_info) }));
            }
        }

        public override void disconnect_unchecked(Node node, Node other, NodePort port) {
            m_pi.SetValue(node, null);
        }

        public virtual IEnumerator<Variable> enumerate_variables() {
            foreach (var attr in m_pi.GetCustomAttributes<ParameterAttribute>()) {
                yield return attr.parameter;
            }
        }
    }

}
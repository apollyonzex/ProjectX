
using System.Collections.Generic;
using GraphNode;
using GraphNode.Editor;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(InvokeNode))]
    public class InvokeNodeEditor : GenericNodeEditor {

        public InvokeNode invoke_node => (InvokeNode)base.node;

        public override void on_graph_open() {
            base.on_graph_open();
            build_stack_frame(false);
            foreach (var pv in view.static_output_ports) {
                build_port_stack_frame(pv, false);
            }
            foreach (var pv in view.dynamic_output_ports) {
                build_port_stack_frame(pv, false);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            build_stack_frame(false);
            foreach (var pv in view.static_output_ports) {
                build_port_stack_frame(pv, false);
            }
            foreach (var pv in view.dynamic_output_ports) {
                build_port_stack_frame(pv, false);
            }
        }

        public virtual void build_stack_frame(bool rise_event) {

        }

        public void build_port_stack_frame(OutputPortView port_view, bool rise_event) {
            if (port_view.port is InvokeWithVariables.IPort iv) {
                if (m_port_stack_frame_dict.TryGetValue(port_view.port, out var list)) {
                    list.Clear();
                } else {
                    list = new List<Variable>();
                    m_port_stack_frame_dict.Add(port_view.port, list);
                }
                var v_iter = enumerate_port_variables(iv) ?? iv.enumerate_variables();
                while (v_iter.MoveNext()) {
                    list.Add(v_iter.Current);
                }
                if (rise_event) {
                    foreach (var conn in port_view.connections) {
                        if (conn.input.node.editor is InvokeNodeEditor child) {
                            child.notify_stack_changed();
                        }
                    }
                }
            }
        }

        public virtual IReadOnlyList<Variable> get_stack_frame() {
            return null;
        }

        public virtual IReadOnlyList<Variable> get_port_stack_frame(NodePort port) {
            if (m_port_stack_frame_dict.TryGetValue(port, out var frame)) {
                return frame;
            }
            return null;
        }

        public virtual PortView get_input_port() { return null; }

        public bool get_parent(out InvokeNodeEditor node, out NodePort output) {
            var input = get_input_port();
            if (input != null) {
                var conn = input.get_first_connection();
                if (conn != null && conn.output.node.editor is InvokeNodeEditor ine) {
                    node = ine;
                    output = conn.output.port;
                    return true;
                }
            }
            node = null;
            output = null;
            return false;
        }

        public virtual bool can_access_graph_variables => true;

        public void notify_stack_changed() {
            view.culled = false;
            m_stack_changed = true;
            foreach (var port in view.static_output_ports) {
                foreach (var conn in port.connections) {
                    if (conn.input.node.editor is InvokeNodeEditor child) {
                        child.notify_stack_changed();
                    }
                }
            }
            foreach (var port in view.dynamic_output_ports) {
                foreach (var conn in port.connections) {
                    if(conn.input.node.editor is InvokeNodeEditor child) {
                        child.notify_stack_changed();
                    }
                }
            }
        }

        public override void on_body_gui() {
            if (m_stack_changed) {
                m_stack_changed = false;
                on_stack_changed();
            }
            base.on_body_gui();
        }

        public override void on_inspector_gui() {
            if (view.graph.window.runtime) {
                GUILayout.BeginVertical(GUI.skin.box);
                break_when_invoke = GUILayout.Toggle(break_when_invoke, "Break When Invoke");
                GUILayout.EndVertical();
            }
            base.on_inspector_gui();
        }

        public override void on_port_desc_gui(PortView port) {
            if (m_port_stack_frame_dict.TryGetValue(port.port, out var list)) {
                var style = GraphResources.styles.node_output_desc;
                foreach (var item in list) {
                    GUILayout.Label($"{item.name}: {item.type.to_string()}", style);
                }
            }
        }

        protected virtual IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            return null;
        }

        protected virtual void on_stack_changed() {}

        public override void on_input_connected(ConnectionView connection) {
            notify_stack_changed();
        }

        public override void on_input_disconnected(ConnectionView connection) {
            notify_stack_changed();
        }

        public bool stack_changed => m_stack_changed;
        private bool m_stack_changed = true;

        protected Dictionary<NodePort, List<Variable>> m_port_stack_frame_dict = new Dictionary<NodePort, List<Variable>>();

        public override Color node_color => m_node_color;

        protected Color m_node_color = new Color32(90, 97, 105, 255);

        public void _set_node_color(Color color) {
            m_node_color = color;
        }

        public bool break_when_invoke = false;

        public virtual object runtime_build_data(IContext context) {
            return null;
        }

        public virtual void runtime_enter(object data) {

        }

        public virtual void runtime_leave() {

        }
    }

    [NodeEditor(typeof(InvokeNodeWithInput))]
    public class InvokeNodeWithInputEditor : InvokeNodeEditor {
        public override PortView get_input_port() {
            return view.static_input_ports[0];
        }
    }


}
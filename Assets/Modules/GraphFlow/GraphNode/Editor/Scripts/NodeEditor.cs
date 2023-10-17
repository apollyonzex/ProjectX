
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public abstract class NodeEditor {
        public abstract void attach(GraphEditor graph, Node node);
        public abstract NodeEditor clone(GraphEditor graph);
        public abstract string node_name { get; }
        public abstract Color node_color { get; }

        public virtual void on_header_gui(GUIStyle style) {
            GUILayout.Label(node_name, style);
        }

        public abstract void on_body_gui();
        public abstract void on_port_desc_gui(PortView port);

        public abstract void on_inspector_enable();
        public abstract void on_inspector_disable();
        public abstract void on_inspector_gui();

        public abstract NodeView view { get; set; }

        public abstract Node node { get; }

        public abstract bool unique { get; }

        public abstract IEnumerator<NodePort> enumerate_dynamic_ports();
        public abstract bool get_dynamic_port_id(NodePort port, out int id);
        public abstract bool get_dynamic_port_by_id(int id, out NodePort port);

        public virtual void on_view_init() {}
        public virtual void on_add_to_graph() { }
        public virtual void on_remove_from_graph() { }
        public virtual void on_graph_open() { }
        public virtual void on_graph_close() { }
        public virtual void on_input_connected(ConnectionView connection) {}
        public virtual void on_input_disconnected(ConnectionView connection) {}
        public virtual void on_output_connected(ConnectionView connection) {}
        public virtual void on_output_disconnected(ConnectionView connection) {}
        public virtual void on_port_connecting(NodePort port, bool connecting) { }

        public virtual void on_double_click() { }
        public virtual void on_node_saving() { }
        public virtual void on_context_menu(GenericMenu menu) { }
        public virtual void on_duplicate_done(List<NodeView> source_nodes, List<NodeView> duplicated_nodes) { }

        public static string build_name(System.Type node_type) {
            var name = node_type.Name;
            if (name.EndsWith("Node", System.StringComparison.Ordinal)) {
                name = name.Substring(0, name.Length - 4);
            }
            return name;
        }

        public abstract bool inspector_enabled { get; }

        public virtual bool has_create_wizard => false;
        public virtual void create_wizard_gui(GraphEditor graph_editor) { }
        public virtual bool validate_create_wizard() { return true; }
    }

    [NodeEditor(typeof(Node))]
    public class GenericNodeEditor : NodeEditor {

        public override void attach(GraphEditor graph, Node node) {
            m_node = node;
            var node_type = node.GetType();
            m_node_name = build_name(node_type);
            if (node_type.GetCustomAttributes(typeof(UniqueAttribute), false).Length > 0) {
                m_unique = true;
            } else {
                m_unique = false;
            }

            m_properties = new List<PropertyEditor>();
            var iter = GraphTypeCache.enumerate_properties(node.GetType());
            while (iter.MoveNext()) {
                var (fi, pe) = iter.Current;
                pe.attach(node, fi, graph, this);
                m_properties.Add(pe);
            }
            m_properties.Sort((a, b) => a.order.CompareTo(b.order));
        }

        public override NodeEditor clone(GraphEditor graph) {
            var editor = (NodeEditor)GetType().GetConstructor(System.Type.EmptyTypes).Invoke(null);
            if (m_node != null) {
                var node = (Node)m_node.GetType().GetConstructor(System.Type.EmptyTypes).Invoke(null);
                clone_to(node);
                editor.attach(graph, node);
            }
            return editor;
        }

        protected virtual void clone_to(Node node) {
            foreach (var pe in m_properties) {
                pe.clone_to(node);
            }
        }

        public override Node node => m_node;

        public override Color node_color => new Color32(90, 97, 105, 255);

        public override string node_name => m_node_name;

        public override bool unique => m_unique;

        protected NodeView m_view;
        public override NodeView view {
            get => m_view;
            set => m_view = value;
        }

        public override void on_graph_open() {
            foreach (var pe in m_properties) {
                pe.on_graph_open();
            }
        }

        public override void on_add_to_graph() {
            foreach (var pe in m_properties) {
                pe.on_node_add_to_graph();
            }
        }

        public override void on_remove_from_graph() {
            foreach (var pe in m_properties) {
                pe.on_node_remove_from_graph();
            }
        }

        public override void on_duplicate_done(List<NodeView> source_nodes, List<NodeView> duplicated_nodes) {
            foreach (var pe in m_properties) {
                pe.on_node_duplicated_done(source_nodes, duplicated_nodes);
            }
        }

        public override void on_body_gui() {
            foreach (var pe in m_properties) {
                pe.on_body_gui();
            }
        }

        public override void on_port_desc_gui(PortView port) {
            
        }

        public override void on_inspector_enable() {
            m_inspector_enabled = true;
            foreach (var pe in m_properties) {
                pe.on_inspector_enable();
            }
        }
        public override void on_inspector_disable() {
            m_inspector_enabled = false;
            foreach (var pe in m_properties) {
                pe.on_inspector_disable();
            }
        }

        public override void on_inspector_gui() {
            foreach (var pe in m_properties) {
                EditorGUI.BeginDisabledGroup(!pe.enabled);
                pe.on_inspector_gui();
                EditorGUI.EndDisabledGroup();
            }
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            yield break;
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            id = 0;
            return false;
        }
        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            port = null;
            return false;
        }

        public virtual bool try_get_property(string raw_name, out PropertyEditor property) {
            foreach (var p in m_properties) {
                if (p.raw_name == raw_name) {
                    property = p;
                    return true;
                }
            }
            property = null;
            return false;
        }


        protected Node m_node;
        protected string m_node_name = "Node";
        private bool m_unique = false;

        protected List<PropertyEditor> m_properties;

        public override bool inspector_enabled => m_inspector_enabled;

        private bool m_inspector_enabled;
    }

}
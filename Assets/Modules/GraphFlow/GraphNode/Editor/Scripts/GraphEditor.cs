
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    public abstract class GraphEditor {
        
        public abstract void attach(Graph graph, GraphView view);
        public abstract void on_open();
        public abstract void on_close();
        public abstract void on_add_node(NodeView node);
        public abstract void on_remove_node(NodeView node);
        public abstract void on_layout();

        public abstract Graph graph { get; }
        public abstract GraphView view { get; }

        public abstract void on_inspector_enable();
        public abstract void on_inspector_disable();
        public abstract void on_inspector_gui();

        public virtual void on_graph_saving() { }
        public virtual void on_graph_saved() { }
        public virtual void on_context_menu(GenericMenu menu) { }

        public virtual Color query_node_port_color(NodePort port) {
            return new Color32(210, 128, 40, 255); 
        }

        public virtual IEnumerator<Type> enumerate_available_node_types() {
            return GraphTypeCache.enumerate_graph_node_types(graph.GetType());
        }

        public virtual IEnumerator<(Type, NodePort)> enumerate_available_node_types(NodePort peer) {
            var i1 = GraphTypeCache.enumerate_graph_node_types(graph.GetType());
            while (i1.MoveNext()) {
                var node_type = i1.Current;
                var i2 = GraphTypeCache.enumerate_node_static_ports(node_type);
                while (i2.MoveNext()) {
                    var port = i2.Current;
                    if (port.can_connect_with(null, null, peer)) {
                        yield return (node_type, port);
                        break;
                    }
                }
            }
        }

        public virtual bool validate_connection(PortView a, PortView b) {
            return true;
        }
    }

    [GraphEditor(typeof(Graph))]
    public class GenericGraphEditor : GraphEditor {
        public override void attach(Graph graph, GraphView view) {
            m_graph = graph;
            m_view = view;
            m_properties = new List<PropertyEditor>();
            var iter = GraphTypeCache.enumerate_properties(graph.GetType());
            while (iter.MoveNext()) {
                var (fi, pe) = iter.Current;
                pe.attach(graph, fi, this, null);
                m_properties.Add(pe);
            }
        }

        public override void on_open() {
            foreach (var node in view.nodes) {
                node.editor.on_graph_open();
            }
        }

        public override void on_close() {
            foreach (var node in view.nodes) {
                node.editor.on_graph_close();
            }
        }

        public override void on_add_node(NodeView node) {
            node.editor.on_add_to_graph();
        }

        public override void on_remove_node(NodeView node) {
            node.editor.on_remove_from_graph();
        }

        public override void on_layout() {

        }

        public override void on_inspector_enable() {
            foreach (var pe in m_properties) {
                pe.on_inspector_enable();
            }
        }
        public override void on_inspector_disable() {
            foreach (var pe in m_properties) {
                pe.on_inspector_disable();
            }
        }

        public override void on_inspector_gui() {
            foreach (var pe in m_properties) {
                pe.on_inspector_gui();
            }
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

        public override Graph graph => m_graph;
        public override GraphView view => m_view;
        protected Graph m_graph;
        protected GraphView m_view;

        protected List<PropertyEditor> m_properties;
    }

}
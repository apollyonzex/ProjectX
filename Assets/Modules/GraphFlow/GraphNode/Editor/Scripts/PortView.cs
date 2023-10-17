
using UnityEngine;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public abstract class PortView {
        public NodeView node { get; }
        public NodePort port { get; }

        public Color color { get; }

        public Rect position { get; protected set; }

        public string name => port.name;

        protected void draw_port() {
            var res = GraphResources.dot_tex;
            GUI.color = color;
            GUI.DrawTexture(position, res.dot);
            GUI.color = node.node_color;
            GUI.DrawTexture(position, res.dot_outer);
        }

        public PortView(NodeView node, NodePort port, Color color) {
            this.node = node;
            this.port = port;
            this.color = color;
        }

        public PortView(NodeView node, NodePort port) : this(node, port, node.graph.editor.query_node_port_color(port)) {

        }

        //public ConnectionView connection { get; set; }

        public HashSet<ConnectionView> connections { get; } = new HashSet<ConnectionView>();

        public void add_connection(ConnectionView conn) {
            connections.Add(conn);
        }

        public void remove_connection(ConnectionView conn) {
            connections.Remove(conn);
        }

        public bool get_connection(InputPortView input_view, out ConnectionView conn) {
            foreach (var e in connections) {
                if (e.input == input_view) {
                    conn = e;
                    return true;
                }
            }
            conn = null;
            return false;
        }

        public bool get_connection(OutputPortView output_view, out ConnectionView conn) {
            foreach (var e in connections) {
                if (e.output == output_view) {
                    conn = e;
                    return true;
                }
            }
            conn = null;
            return false;
        }

        public ConnectionView[] get_connection_array() {
            var ret = new ConnectionView[connections.Count];
            var index = -1;
            foreach (var conn in connections) {
                ret[++index] = conn;
            }
            return ret;
        }

        public ConnectionView get_first_connection() {
            var iter = connections.GetEnumerator();
            return iter.MoveNext() ? iter.Current : null;
        }

        public bool connected => connections.Count != 0;
    }

    public class InputPortView : PortView {
        public InputPortView(NodeView node, NodePort port, Color color) : base(node, port, color) {

        }

        public InputPortView(NodeView node, NodePort port) : base(node, port) {

        }

        public void on_gui_layout(NodeView node) {
            GUILayout.Label(name, GraphResources.styles.node_input_port);
            if (Event.current.type == EventType.Repaint) {
                var rect = GUILayoutUtility.GetLastRect();
                position = new Rect(rect.position + new Vector2(-16, 4), new Vector2(16, 16));
                draw_port();
            }
            
        }
    }

    public class OutputPortView : PortView {
        public OutputPortView(NodeView node, NodePort port, Color color) : base(node, port, color) {

        }

        public OutputPortView(NodeView node, NodePort port) : base(node, port) {

        }

        public void on_gui_layout(NodeView node) {
            GUILayout.Label(name, GraphResources.styles.node_output_port);
            if (Event.current.type == EventType.Repaint) {
                var rect = GUILayoutUtility.GetLastRect();
                var res = GraphResources.dot_tex;
                position = new Rect(rect.position + new Vector2(rect.width, 4), new Vector2(16, 16));
                draw_port();
            }
        }
    }

}
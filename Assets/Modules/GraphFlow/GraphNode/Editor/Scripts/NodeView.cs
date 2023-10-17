
using UnityEngine;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public class NodeView {

        static readonly GUILayoutOption MIN_WIDTH = GUILayout.MinWidth(64);
        static readonly GUILayoutOption MIN_HEIGHT = GUILayout.MinHeight(64);

        public NodeView(GraphView graph, NodeEditor editor) {
            this.graph = graph;
            this.editor = editor;
            editor.view = this;
            var iter = GraphTypeCache.enumerate_node_static_ports(editor.node.GetType());
            while (iter.MoveNext()) {
                var port = iter.Current;
                var color = graph.editor.query_node_port_color(port);
                if (port.io.is_input()) {
                    static_input_ports.Add(new InputPortView(this, port, color));
                } else {
                    static_output_ports.Add(new OutputPortView(this, port, color));
                }
            }

            var dps = editor.enumerate_dynamic_ports();
            while (dps.MoveNext()) {
                var port = dps.Current;
                var color = graph.editor.query_node_port_color(port);
                if (port.io.is_input()) {
                    dynamic_input_ports.Add(new InputPortView(this, port, color));
                } else {
                    dynamic_output_ports.Add(new OutputPortView(this, port, color));
                }
            }

            editor.on_view_init();
        }

        public bool culled { get; set; } = false;

        public bool size_changed {
            get => m_size_changed;
            set {
                if (value) {
                    m_size_changed = true;
                }
            }
        }

        public Vector2 position { 
            get => editor.node.position;
            set => editor.node.position = value;
        }

        public Vector2 size { get; set; }

        public bool selected { get; set; } = false;

        public Color node_color => editor.node_color;

        public NodeEditor editor { get; }
        public GraphView graph { get; }

        public void on_gui() {
            var styles = GraphResources.styles;

            GUILayout.BeginArea(new Rect(m_pos_in_clipped_gui, new Vector2(2048, 2048)));
            GUILayout.BeginHorizontal();

            if (selected) {
                GUI.color = node_color;
                GUILayout.BeginVertical(styles.node_selected_body);
                GUI.color = Color.white;
                GUILayout.BeginVertical(styles.node_selected_outer, MIN_WIDTH, MIN_HEIGHT);
            } else {
                GUI.color = node_color;
                GUILayout.BeginVertical(styles.node_body, MIN_WIDTH, MIN_HEIGHT);
            }



            GUI.color = Color.white;
            editor.on_header_gui(styles.node_header);

            // content
            editor.on_body_gui();

            // ports
            GUILayout.BeginHorizontal();
            // input ports;
            GUILayout.BeginVertical();
            foreach (var port in static_input_ports) {
                port.on_gui_layout(this);
                editor.on_port_desc_gui(port);
            }
            foreach (var port in dynamic_input_ports) {
                port.on_gui_layout(this);
                editor.on_port_desc_gui(port);
            }
            GUILayout.EndVertical();

            GUILayout.Space(8);

            // output ports;
            GUILayout.BeginVertical();
            foreach (var port in static_output_ports) {
                port.on_gui_layout(this);
                editor.on_port_desc_gui(port);
            }
            foreach (var port in dynamic_output_ports) {
                port.on_gui_layout(this);
                editor.on_port_desc_gui(port);
            }
            GUILayout.EndVertical();

            // end ports
            GUILayout.EndHorizontal();

            // end node
            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint) {
                size = GUILayoutUtility.GetLastRect().size;
                if (m_size_changed) {
                    m_size_changed = false;
                    update_culled();
                }
            }

            if (selected) {
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        internal void on_layout_for_recull() {
            m_pos_in_clipped_gui = graph.local_to_clipped_gui(position);
            if (!m_size_changed) {
                update_culled();
            }
        }

        internal bool contains(Vector2 point_in_clipped_gui) {
            return new Rect(m_pos_in_clipped_gui, size).Contains(point_in_clipped_gui);
        }

        internal bool overlaps(Rect rect_in_clipped_gui) {
            return rect_in_clipped_gui.Overlaps(new Rect(m_pos_in_clipped_gui, size));
        }

        internal Vector2 pos_in_clipped_gui => m_pos_in_clipped_gui;

        Vector2 m_pos_in_clipped_gui;
        bool m_size_changed = true;

        private void update_culled() {
            var rect = new Rect(m_pos_in_clipped_gui, size);
            culled = !graph.local_rect.Overlaps(rect);
        }

        public List<InputPortView> static_input_ports = new List<InputPortView>();
        public List<OutputPortView> static_output_ports = new List<OutputPortView>();

        public List<InputPortView> dynamic_input_ports = new List<InputPortView>();
        public List<OutputPortView> dynamic_output_ports = new List<OutputPortView>();

        public IEnumerator<PortView> enumerate_ports() {
            foreach (var port in static_input_ports) {
                yield return port;
            }
            foreach (var port in dynamic_input_ports) {
                yield return port;
            }
            foreach (var port in static_output_ports) {
                yield return port;
            }
            foreach (var port in dynamic_output_ports) {
                yield return port;
            }
        }

        public PortView pick_port(Vector2 point_in_clipped_gui) {
            var pt = point_in_clipped_gui - m_pos_in_clipped_gui;
            foreach (var port in static_input_ports) {
                if (port.position.Contains(pt)) {
                    return port;
                }
            }
            foreach (var port in static_output_ports) {
                if (port.position.Contains(pt)) {
                    return port;
                }
            }
            foreach (var port in dynamic_input_ports) {
                if (port.position.Contains(pt)) {
                    return port;
                }
            }
            foreach (var port in dynamic_output_ports) {
                if (port.position.Contains(pt)) {
                    return port;
                }
            }
            return null;
        }

        public PortView find_static_port(NodePort port) {
            if (port.io.is_input()) {
                foreach (var sp in static_input_ports) {
                    if (sp.port.Equals(port)) {
                        return sp;
                    }
                }
            } else {
                foreach (var sp in static_output_ports) {
                    if (sp.port.Equals(port)) {
                        return sp;
                    }
                }
            }
            return null;
        }

        public PortView find_dynamic_port(NodePort port) {
            if (port.io.is_input()) {
                foreach (var dp in dynamic_input_ports) {
                    if (dp.port == port) {
                        return dp;
                    }
                }
            } else {
                foreach (var dp in dynamic_output_ports) {
                    if (dp.port == port) {
                        return dp;
                    }
                }
            }
            return null;
        }

        public InputPortView find_input_port(NodePort port) {
            if (port.is_static) {
                foreach (var pv in static_input_ports) {
                    if (pv.port.Equals(port)) {
                        return pv;
                    }
                }
            } else {
                foreach (var pv in dynamic_input_ports) {
                    if (pv.port == port) {
                        return pv;
                    }
                }
            }
            return null;
        }

        public OutputPortView find_output_port(NodePort port) {
            if (port.is_static) {
                foreach (var pv in static_output_ports) {
                    if (pv.port.Equals(port)) {
                        return pv;
                    }
                }
            } else {
                foreach (var pv in dynamic_output_ports) {
                    if (pv.port == port) {
                        return pv;
                    }
                }
            }
            return null;
        }

        public PortView find_port(NodeEditor other_node, NodePort other_port) {
            if (other_port.is_static) {
                return find_static_port(other_port);
            }
            if (other_node.get_dynamic_port_id(other_port, out var id)) {
                if (editor.get_dynamic_port_by_id(id, out other_port)) {
                    return find_dynamic_port(other_port);
                }
            }
            return null;
        }

        public PortView add_dynamic_port(NodePort port) {
            var color = graph.editor.query_node_port_color(port);
            if (port.io.is_input()) {

                var pv = new InputPortView(this, port, color);
                dynamic_input_ports.Add(pv);
                size_changed = true;
                return pv;

            } else {

                var pv = new OutputPortView(this, port, color);
                dynamic_output_ports.Add(pv);
                size_changed = true;
                return pv;

            }
        }

        public PortView remove_dynamic_port(NodePort port) {
            if (port.io.is_input()) {
                for (int i = 0; i < dynamic_input_ports.Count; ++i) {
                    var pv = dynamic_input_ports[i];
                    if (pv.port == port) {
                        dynamic_input_ports.RemoveAt(i);
                        size_changed = true;
                        return pv;
                    }
                }
            } else {
                for (int i = 0; i < dynamic_output_ports.Count; ++i) {
                    var pv = dynamic_input_ports[i];
                    if (pv.port == port) {
                        dynamic_output_ports.RemoveAt(i);
                        size_changed = true;
                        return pv;
                    }
                }
            }
            return null;
        }

        public OutputPortView get_output_port_by_index(int index) {
            if (index < static_output_ports.Count) {
                return static_output_ports[index];
            }
            index -= static_output_ports.Count;
            if (index < dynamic_output_ports.Count) {
                return dynamic_output_ports[index];
            }
            return null;
        }
    }

}
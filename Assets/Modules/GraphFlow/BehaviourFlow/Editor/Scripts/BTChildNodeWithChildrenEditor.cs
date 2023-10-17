
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourFlow.Editor {
    [NodeEditor(typeof(BTChildNodeWithChildren))]
    public class BTChildNodeWithChildrenEditor : BTNodeEditor {

        public new BTChildNodeWithChildren node => base.node as BTChildNodeWithChildren;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            m_graph = graph;
            try_get_property("children", out var pe);
            m_children_editor = pe as BTChildrenEditor;
            for (int i = 0; i < m_children_editor.target.Count; ++i) {
                m_children_editor.target[i].index = i;
            }
            m_incoming_port = new BTChildNodePort { index = m_children_editor.target.Count };
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            if (id >= m_children_editor.target.Count) {
                for (int i = m_children_editor.target.Count; i <= id; ++i) {
                    var p = new BTChildNodePort { index = i };
                    m_children_editor.target.Add(p);
                    view.dynamic_output_ports.Insert(i, new OutputPortView(view, p, view.graph.editor.query_node_port_color(p)));
                }
            }
            port = m_children_editor.target[id];
            return true;
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            for (int i = 0; i < m_children_editor.target.Count; ++i) {
                if (m_children_editor.target[i] == port) {
                    id = i;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            foreach (var port in m_children_editor.target) {
                yield return port;
            }
            yield return m_incoming_port;
        }

        public override void on_output_connected(ConnectionView connection) {
            if (view.graph.undo.operating) {
                return;
            }

            if (connection.output.port == m_incoming_port) {
                var cmd = new AddPort {
                    editor = this,
                };
                cmd.port = m_incoming_port;
                cmd.incoming_port = new BTChildNodePort { index = m_children_editor.target.Count + 1 };
                cmd.incoming_port_view = new OutputPortView(view, cmd.incoming_port, view.graph.editor.query_node_port_color(m_incoming_port));
                cmd.redo();
                view.graph.undo.record(cmd);
            }
        }

        public override void on_output_disconnected(ConnectionView connection) {
            if (view.graph.undo.operating || is_port_connecting) {
                return;
            }
            if (connection.output.port is BTChildNodePort np) {
                var cmd = new RemovePort {
                    editor = this,
                    port = np,
                    port_view = view.dynamic_output_ports[np.index],
                };
                cmd.redo();
                view.graph.undo.record(cmd);
            }
        }

        public override void on_port_connecting(NodePort port, bool connecting) {
            if (port is BTChildNodePort np) {
                if (connecting) {
                    if (++m_port_connecting_count == 1) {
                        view.graph.undo.begin_group();
                    }
                } else {
                    if (--m_port_connecting_count == 0) {
                        if (np.value == null) {
                            var cmd = new RemovePort {
                                editor = this,
                                port = np,
                                port_view = view.dynamic_output_ports[np.index],
                            };
                            cmd.redo();
                            view.graph.undo.record(cmd);
                        }
                        view.graph.undo.end_group();
                    }
                }
            }
        }

        public bool is_port_connecting => m_port_connecting_count != 0;

        private int m_port_connecting_count = 0;

        public override void on_context_menu(GenericMenu menu) {
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Reorder Children in Vertical"), false, m_children_editor.reorder_in_vertical);
        }

        public override void on_duplicate_done(List<NodeView> source_nodes, List<NodeView> duplicated_nodes) {
            for (int i = 0; i < m_children_editor.target.Count; ) {
                if (m_children_editor.target[i].value == null) {
                    m_children_editor.target.RemoveAt(i);
                    view.dynamic_output_ports.RemoveAt(i);
                } else {
                    ++i;
                }
            }
            view.size_changed = true;
            reassign_port_indices(0);
        }

        private void reassign_port_indices(int start) {
            for (int i = start; i < m_children_editor.target.Count; ++i) {
                m_children_editor.target[i].index = i;
            }
            m_incoming_port.index = m_children_editor.target.Count;
        }

        protected BTChildrenEditor m_children_editor;
        protected BTChildNodePort m_incoming_port;

        protected GraphEditor m_graph;

        private class AddPort : GraphUndo.ICommand {
            public BTChildNodeWithChildrenEditor editor;
            public BTChildNodePort port;
            public BTChildNodePort incoming_port;
            public OutputPortView incoming_port_view;

            public int dirty_count => 1;

            public void redo() {
                editor.node.children.Add(port);
                editor.m_incoming_port = incoming_port;
                editor.view.dynamic_output_ports.Add(incoming_port_view);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.children.RemoveAt(port.index);
                editor.m_incoming_port = port;
                editor.view.dynamic_output_ports.RemoveAt(incoming_port.index);
                editor.view.size_changed = true;
            }
        }

        private class RemovePort : GraphUndo.ICommand {
            public BTChildNodeWithChildrenEditor editor;
            public BTChildNodePort port;
            public OutputPortView port_view;

            public int dirty_count => 1;

            public void redo() {
                var index = port.index;
                editor.node.children.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                editor.view.size_changed = true;
                editor.reassign_port_indices(index);
            }

            public void undo() {
                var index = port.index;
                editor.node.children.Insert(index, port);
                editor.view.dynamic_output_ports.Insert(index, port_view);
                editor.view.size_changed = true;
                editor.reassign_port_indices(index);
            }
        }
    }
}
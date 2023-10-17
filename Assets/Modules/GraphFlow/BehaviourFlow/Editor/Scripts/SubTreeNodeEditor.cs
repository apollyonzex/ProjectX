
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

namespace BehaviourFlow.Editor {
    [NodeEditor(typeof(Nodes.SubTreeNode))]
    public class SubTreeNodeEditor : BTNodeEditor {

        public new Nodes.SubTreeNode node => base.node as Nodes.SubTreeNode;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("target", out var pe)) {
                pe.on_changed += on_target_changed;
            }

            var st = this.node;
            if (st.target != null) {
                var bt = st.target.graph;

                if (st.ports != null) {
                    for (int i = 0; i < st.ports.Count;) {
                        if (!bt.externals.ContainsKey(st.ports[i].name)) {
                            st.ports.RemoveAt(i);
                        } else {
                            ++i;
                        }
                    }
                    foreach (var kvp in bt.externals) {
                        var idx = st.binary_search_port(kvp.Key);
                        if (idx < 0) {
                            st.ports.Insert(~idx, new Nodes.SubTreeNode.ExternalPort { _name = kvp.Key });
                        }
                    }
                } else {
                    st.ports = new List<Nodes.SubTreeNode.ExternalPort>(bt.externals.Count);
                    foreach (var kvp in bt.externals) {
                        st.ports.Add(new Nodes.SubTreeNode.ExternalPort { _name = kvp.Key });
                    }
                    st.ports.Sort((a, b) => a._name.CompareTo(b._name));
                }

            } else if (st.ports == null) {
                st.ports = new List<Nodes.SubTreeNode.ExternalPort>();
            } else {
                st.ports.Clear();
            }
            
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            return node.ports.GetEnumerator();
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            var node = this.node;
            if (id >= 0 && id < node.ports.Count) {
                port = node.ports[id];
                return true;
            }
            port = null;
            return false;
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            var node = this.node;
            for (int i = 0; i < node.ports.Count; ++i) {
                var p = node.ports[i];
                if (p == port) {
                    id = i;
                    return true;
                }
            }
            id = -1;
            return false;
        }

        private void on_target_changed(PropertyEditor pe, bool by_user) {
            update_ports();
        }

        private void update_ports() {
            var node = this.node;
            if (node.target == null) {
                for (int i = node.ports.Count - 1; i >= 0; --i) {
                    var cmd = new RemovePort(this, i);
                    cmd.redo();
                    view.graph.undo.record(cmd);
                }
            } else {
                var bt = node.target.graph;
                for (int i = node.ports.Count - 1; i >= 0; --i) {
                    if (!bt.externals.ContainsKey(node.ports[i].name)) {
                        var cmd = new RemovePort(this, i);
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }

                foreach (var kvp in bt.externals) {
                    var idx = node.binary_search_port(kvp.Key);
                    if (idx < 0) {
                        var port = new Nodes.SubTreeNode.ExternalPort { _name = kvp.Key };
                        var cmd = new InsertPort {
                            editor = this,
                            index = ~idx,
                            port = port,
                            view = new OutputPortView(view, port, view.graph.editor.query_node_port_color(port)),
                        };
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }
            }
        }

        private class RemovePort : GraphUndo.ICommand {
            public SubTreeNodeEditor editor;
            public int index;
            public Nodes.SubTreeNode.ExternalPort port;
            public OutputPortView view;
            public ConnectionView conn;

            public RemovePort(SubTreeNodeEditor editor, int index) {
                this.editor = editor;
                this.index = index;
                port = editor.node.ports[index];
                view = editor.view.dynamic_output_ports[index];
                conn = view.get_first_connection();
            }

            public int dirty_count => 1;

            public void redo() {
                editor.node.ports.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                if (conn != null) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.ports.Insert(index, port);
                editor.view.dynamic_output_ports.Insert(index, view);
                if (conn != null) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }
        }

        private class InsertPort : GraphUndo.ICommand {
            public SubTreeNodeEditor editor;
            public int index;
            public Nodes.SubTreeNode.ExternalPort port;
            public OutputPortView view;

            public int dirty_count => 1;

            public void redo() {
                editor.node.ports.Insert(index, port);
                editor.view.dynamic_output_ports.Insert(index, view);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.ports.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                editor.view.size_changed = true;
            }
        }
    }
}

using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

namespace DialogFlow.Editor.NodeEditors {

    [NodeEditor(typeof(Nodes.JumpToDialog))]
    public class JumpToDialogEditor : DialogNodeEditor {
        public new Nodes.JumpToDialog node => base.node as Nodes.JumpToDialog;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("target", out var pe)) {
                pe.on_changed += on_target_changed;
            }

            var this_node = this.node;
            if (this_node.target != null) {
                var dialog = this_node.target.graph;

                if (this_node.ports != null) {
                    for (int i = 0; i < this_node.ports.Count;) {
                        if (!dialog.externals.ContainsKey(this_node.ports[i].name)) {
                            this_node.ports.RemoveAt(i);
                        } else {
                            ++i;
                        }
                    }
                    foreach (var kvp in dialog.externals) {
                        var idx = this_node.binary_search_port(kvp.Key);
                        if (idx < 0) {
                            this_node.ports.Insert(~idx, new Nodes.JumpToDialog.ExternalPort { _name = kvp.Key });
                        }
                    }
                } else {
                    this_node.ports = new List<Nodes.JumpToDialog.ExternalPort>(dialog.externals.Count);
                    foreach (var kvp in dialog.externals) {
                        this_node.ports.Add(new Nodes.JumpToDialog.ExternalPort { _name = kvp.Key });
                    }
                    this_node.ports.Sort((a, b) => a._name.CompareTo(b._name));
                }

                if (this_node.option_states != null) {
                    for (int i = 0; i < this_node.option_states.Count;) {
                        if (!dialog.option_state_externals.ContainsKey(this_node.option_states[i].name)) {
                            this_node.option_states.RemoveAt(i);
                        } else {
                            ++i;
                        }
                    }
                    foreach (var kvp in dialog.option_state_externals) {
                        var idx = this_node.binary_search_option_state(kvp.Key);
                        if (idx < 0) {
                            this_node.option_states.Insert(~idx, new Nodes.JumpToDialog.OptionStateExternalPort { _name = kvp.Key });
                        }
                    }
                } else {
                    this_node.option_states = new List<Nodes.JumpToDialog.OptionStateExternalPort>(dialog.option_state_externals.Count);
                    foreach (var kvp in dialog.option_state_externals) {
                        this_node.option_states.Add(new Nodes.JumpToDialog.OptionStateExternalPort { _name = kvp.Key });
                    }
                    this_node.option_states.Sort((a, b) => a._name.CompareTo(b._name));
                }

                if (this_node.options != null) {
                    for (int i = 0; i < this_node.options.Count;) {
                        if (!dialog.options_externals.ContainsKey(this_node.options[i].name)) {
                            this_node.options.RemoveAt(i);
                        } else {
                            ++i;
                        }
                    }
                    foreach (var kvp in dialog.options_externals) {
                        var idx = this_node.binary_search_options(kvp.Key);
                        if (idx < 0) {
                            this_node.options.Insert(~idx, new Nodes.JumpToDialog.OptionsExternalPort { _name = kvp.Key });
                        }
                    }
                } else {
                    this_node.options = new List<Nodes.JumpToDialog.OptionsExternalPort>(dialog.options_externals.Count);
                    foreach (var kvp in dialog.options_externals) {
                        this_node.options.Add(new Nodes.JumpToDialog.OptionsExternalPort { _name = kvp.Key });
                    }
                    this_node.options.Sort((a, b) => a._name.CompareTo(b._name));
                }

            } else {
                if (this_node.ports == null) {
                    this_node.ports = new List<Nodes.JumpToDialog.ExternalPort>();
                } else {
                    this_node.ports.Clear();
                }
                if (this_node.option_states == null) {
                    this_node.option_states = new List<Nodes.JumpToDialog.OptionStateExternalPort>();
                } else {
                    this_node.option_states.Clear();
                }
                if (this_node.options == null) {
                    this_node.options = new List<Nodes.JumpToDialog.OptionsExternalPort>();
                } else {
                    this_node.options.Clear();
                }
            }

        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            var node = this.node;
            foreach (var port in node.option_states) {
                yield return port;
            }
            foreach (var port in node.ports) {
                yield return port;
            }
            foreach (var port in node.options) {
                yield return port;
            }
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            var node = this.node;
            if (id > 0) {
                if (id <= node.ports.Count) {
                    port = node.ports[id - 1];
                    return true;
                }
                id -= node.ports.Count;
                if (id <= node.options.Count) {
                    port = node.options[id - 1];
                    return true;
                }
            } else if (id < 0) {
                id = -id - 1;
                if (id < node.option_states.Count) {
                    port = node.option_states[id];
                    return true;
                }
            }
            port = null;
            return false;
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            var node = this.node;
            if (port is Nodes.JumpToDialog.ExternalPort ep) {
                for (int i = 0; i < node.ports.Count; ++i) {
                    if (ep == node.ports[i]) {
                        id = i + 1;
                        return true;
                    }
                }
            } else if (port is Nodes.JumpToDialog.OptionsExternalPort oep) {
                for (int i = 0; i < node.options.Count; ++i) {
                    if (oep == node.options[i]) {
                        id = i + node.ports.Count + 1;
                        return true;
                    }
                }
            } else if (port is Nodes.JumpToDialog.OptionStateExternalPort osep) {
                for (int i = 0; i < node.option_states.Count; ++i) {
                    if (osep == node.option_states[i]) {
                        id = -(i + 1);
                        return true;
                    }
                }
            }
            id = 0;
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
                for (int i = node.option_states.Count - 1; i >= 0; --i) {
                    var cmd = new RemoveOptionStatePort(this, i);
                    cmd.redo();
                    view.graph.undo.record(cmd);
                }
                for (int i = node.options.Count - 1; i >= 0; --i) {
                    var cmd = new RemoveOptionsPort(this, i);
                    cmd.redo();
                    view.graph.undo.record(cmd);
                }
            } else {
                var dialog = node.target.graph;
                for (int i = node.ports.Count - 1; i >= 0; --i) {
                    if (!dialog.externals.ContainsKey(node.ports[i].name)) {
                        var cmd = new RemovePort(this, i);
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }

                foreach (var kvp in dialog.externals) {
                    var idx = node.binary_search_port(kvp.Key);
                    if (idx < 0) {
                        var port = new Nodes.JumpToDialog.ExternalPort { _name = kvp.Key };
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

                for (int i = node.option_states.Count - 1; i >= 0; --i) {
                    if (!dialog.option_state_externals.ContainsKey(node.option_states[i].name)) {
                        var cmd = new RemoveOptionStatePort(this, i);
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }

                foreach (var kvp in dialog.option_state_externals) {
                    var idx = node.binary_search_option_state(kvp.Key);
                    if (idx < 0) {
                        var port = new Nodes.JumpToDialog.OptionStateExternalPort { _name = kvp.Key };
                        var cmd = new InsertOptionStatePort {
                            editor = this,
                            index = ~idx,
                            port = port,
                            view = new InputPortView(view, port, view.graph.editor.query_node_port_color(port)),
                        };
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }

                for (int i = node.options.Count - 1; i >= 0; --i) {
                    if (!dialog.options_externals.ContainsKey(node.options[i].name)) {
                        var cmd = new RemoveOptionsPort(this, i);
                        cmd.redo();
                        view.graph.undo.record(cmd);
                    }
                }

                foreach (var kvp in dialog.options_externals) {
                    var idx = node.binary_search_options(kvp.Key);
                    if (idx < 0) {
                        var port = new Nodes.JumpToDialog.OptionsExternalPort { _name = kvp.Key };
                        var cmd = new InsertOptionsPort {
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
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.ExternalPort port;
            public OutputPortView view;
            public ConnectionView conn;

            public RemovePort(JumpToDialogEditor editor, int index) {
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

        private class RemoveOptionsPort : GraphUndo.ICommand {
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.OptionsExternalPort port;
            public OutputPortView view;
            public ConnectionView conn;

            public RemoveOptionsPort(JumpToDialogEditor editor, int index) {
                this.editor = editor;
                this.index = index;
                port = editor.node.options[index];
                view = editor.view.dynamic_output_ports[index];
                conn = view.get_first_connection();
            }

            public int dirty_count => 1;

            public void redo() {
                editor.node.options.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(editor.node.ports.Count + index);
                if (conn != null) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.options.Insert(index, port);
                editor.view.dynamic_output_ports.Insert(editor.node.ports.Count + index, view);
                if (conn != null) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }
        }

        private class RemoveOptionStatePort : GraphUndo.ICommand {
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.OptionStateExternalPort port;
            public InputPortView view;
            public ConnectionView conn;

            public RemoveOptionStatePort(JumpToDialogEditor editor, int index) {
                this.editor = editor;
                this.index = index;
                port = editor.node.option_states[index];
                view = editor.view.dynamic_input_ports[index];
                conn = view.get_first_connection();
            }

            public int dirty_count => 1;

            public void redo() {
                editor.node.option_states.RemoveAt(index);
                editor.view.dynamic_input_ports.RemoveAt(index);
                if (conn != null) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.option_states.Insert(index, port);
                editor.view.dynamic_input_ports.Insert(index, view);
                if (conn != null) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }
        }

        private class InsertPort : GraphUndo.ICommand {
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.ExternalPort port;
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

        private class InsertOptionsPort : GraphUndo.ICommand {
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.OptionsExternalPort port;
            public OutputPortView view;

            public int dirty_count => 1;

            public void redo() {
                editor.node.options.Insert(index, port);
                editor.view.dynamic_output_ports.Insert(editor.node.ports.Count + index, view);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.options.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(editor.node.ports.Count + index);
                editor.view.size_changed = true;
            }
        }

        private class InsertOptionStatePort : GraphUndo.ICommand {
            public JumpToDialogEditor editor;
            public int index;
            public Nodes.JumpToDialog.OptionStateExternalPort port;
            public InputPortView view;

            public int dirty_count => 1;

            public void redo() {
                editor.node.option_states.Insert(index, port);
                editor.view.dynamic_input_ports.Insert(index, view);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.option_states.RemoveAt(index);
                editor.view.dynamic_input_ports.RemoveAt(index);
                editor.view.size_changed = true;
            }
        }

    }
}
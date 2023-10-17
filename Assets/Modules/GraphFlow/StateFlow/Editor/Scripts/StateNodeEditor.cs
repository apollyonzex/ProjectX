
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using GraphNode.Editor;
using InvokeFlow;
using GraphNode;

namespace StateFlow.Editor {

    [NodeEditor(typeof(StateNode))]
    public class StateNodeEditor : StateNodeBaseEditor {

        public new StateNode node => m_node as StateNode;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var this_node = this.node;
            if (graph is StateGraphEditor ge) {
                if (this_node.events.Count == 0 && ge.state_events.Count != 0) {
                    foreach (var kvp in ge.state_events) {
                        this_node.events.Add(kvp.Key, new StateNode.EventPort { node = kvp.Value.node });
                    }
                }
            }
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            foreach (var kvp in node.events) {
                yield return kvp.Value;
            }
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            id = 0;
            foreach (var kvp in node.events) {
                if (kvp.Value == port) {
                    return true;
                }
                ++id;
            }
            return false;
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            port = node.events.Values[id];
            return true;
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.on_state_event_changed += on_state_event_changed;
                ge.on_state_event_parameters_changed += on_state_event_parameters_changed;
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.on_state_event_changed += on_state_event_changed;
                ge.on_state_event_parameters_changed += on_state_event_parameters_changed;
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.on_state_event_changed -= on_state_event_changed;
                ge.on_state_event_parameters_changed -= on_state_event_parameters_changed;
            }
        }

        private void on_state_event_parameters_changed(int index) {
            build_port_stack_frame(view.dynamic_output_ports[index], true);
        }

        private void on_state_event_changed(StateEventNodeEditor see, bool added) {
            if (!view.graph.undo.operating) {
                var node = this.node;
                if (added) {
                    var port = new StateNode.EventPort { node = see.node };
                    node.events.Add(see.node.name, port);
                    var port_view = new OutputPortView(view, port);
                    var index = node.events.IndexOfKey(see.node.name);
                    view.dynamic_output_ports.Insert(index, port_view);
                    view.graph.undo.record(new AddEventPort { editor = this, port = port, view = port_view, index = index });
                } else {
                    var index = node.events.IndexOfKey(see.node.name);
                    var port = node.events.Values[index];
                    var port_view = view.dynamic_output_ports[index];
                    var conn = port_view.get_first_connection();
                    if (conn != null) {
                        view.graph.undo.record(new GraphUndo.DestroyConnection(conn));
                        view.graph.remove_connection_unchecked(conn);
                    }
                    view.graph.undo.record(new RemoveEventPort { editor = this, port = port, view = port_view, index = index });
                    node.events.RemoveAt(index);
                    view.dynamic_output_ports.RemoveAt(index);
                }
                view.size_changed = true;
            }
        }

        public override void build_stack_frame(bool rise_event) {
            var node = this.node;
            m_stack_frame.Clear();
            var iter = node.variables.enumerate_valid_variables();
            while (iter.MoveNext()) {
                m_stack_frame.Add(iter.Current);
            }

            node.stack_frame = new int[m_stack_frame.Count];
            for (int i = 0; i < node.stack_frame.Length; ++i) {
                node.stack_frame[i] = m_stack_frame[i].value_in_stack;
            }

            if (rise_event) {
                notify_stack_changed();
            }
        }

        public override IReadOnlyList<Variable> get_stack_frame() {
            return m_stack_frame;
        }

        public override void on_header_gui(GUIStyle style) {
            var node = this.node;
            GUILayout.Label($"State {node.name}", style);
        }

        /*
        public override void on_body_gui() {
            base.on_body_gui();
            foreach (var item in m_stack_frame) {
                GUILayout.Label(VariablesEditor.get_variable_display(item));
            }
        }
        */

        protected List<Variable> m_stack_frame = new List<Variable>();

        #region create wizard
        public override bool has_create_wizard => true;

        public override void create_wizard_gui(GraphEditor graph_editor) {
            var node = this.node;
            node.name = EditorGUILayout.TextField("Name", node.name)?.Trim();
            m_create_ok = false;
            if (!string.IsNullOrEmpty(node.name)) {
                if (graph_editor is StateGraphEditor ge) {
                    if (!ge.state_dict.ContainsKey(node.name)) {
                        m_create_ok = true;
                    } else {
                        EditorGUILayout.HelpBox($"'{node.name}' already exist", MessageType.Warning);
                    }
                }
            }
        }

        public override bool validate_create_wizard() {
            return m_create_ok;
        }

        bool m_create_ok = false;
        #endregion

        #region undo commands

        class AddEventPort : GraphUndo.ICommand {
            public StateNodeEditor editor;
            public StateNode.EventPort port;
            public OutputPortView view;
            public int index;

            public int dirty_count => 1;

            public void redo() {
                editor.node.events.Add(port.node.name, port);
                editor.view.dynamic_output_ports.Insert(index, view);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.events.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                editor.view.size_changed = true;
            }
        }

        class RemoveEventPort : GraphUndo.ICommand {
            public StateNodeEditor editor;
            public StateNode.EventPort port;
            public OutputPortView view;
            public int index;

            public int dirty_count => 1;

            public void redo() {
                editor.node.events.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                editor.view.size_changed = true;
            }

            public void undo() {
                editor.node.events.Add(port.node.name, port);
                editor.view.dynamic_output_ports.Insert(index, view);
                editor.view.size_changed = true;
            }
        }

        #endregion
    }
}
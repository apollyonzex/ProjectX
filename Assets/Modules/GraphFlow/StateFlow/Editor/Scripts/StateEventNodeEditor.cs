
using GraphNode;
using GraphNode.Editor;
using InvokeFlow;
using InvokeFlow.Editor;

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;



namespace StateFlow.Editor {

    [NodeEditor(typeof(StateEventNode))]
    public class StateEventNodeEditor : InvokeNodeEditor {

        public override Color node_color => new Color32(94, 90, 55, 255);

        public new StateEventNode node => m_node as StateEventNode;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.state_events.Add(node.name, this);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.notify_state_event_added(this);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor is StateGraphEditor ge) {
                ge.notify_state_event_removed(this);
            }
        }

        public override void build_stack_frame(bool rise_event) {
            var node = this.node;
            node.available_parameters.Clear();
            var iter = node.parameters.enumerate_valid_variables();
            while (iter.MoveNext()) {
                node.available_parameters.Add(iter.Current as Parameter);
            }
            if (rise_event && view.graph.editor is StateGraphEditor ge) {
                ge.notify_state_event_parameters_changed(this);
            }
        }

        public override void on_body_gui() {
            var node = this.node;
            GUILayout.Label($"[{node.name}]");
            foreach (var p in node.available_parameters) {
                GUILayout.Label(ParametersEditor.get_parameter_display(p));
            }
        }

        #region create wizard
        public override bool has_create_wizard => true;

        public override void create_wizard_gui(GraphEditor graph_editor) {
            var node = m_node as StateEventNode;
            node.name = EditorGUILayout.TextField("Name", node.name)?.Trim();
            m_create_ok = false;
            if (!string.IsNullOrEmpty(node.name)) {
                if (graph_editor is StateGraphEditor ge) {
                    if (!ge.graph.entry.events.ContainsKey(node.name)) {
                        m_create_ok = true;
                    } else {
                        EditorGUILayout.HelpBox($"[{node.name}] already exist", MessageType.Warning);
                    }
                }
            }
        }

        public override bool validate_create_wizard() {
            return m_create_ok;
        }

        bool m_create_ok = false;
        #endregion
    }
}
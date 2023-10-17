
using GraphNode.Editor;
using UnityEditor;
using UnityEngine;

namespace DialogFlow.Editor.NodeEditors {

    [NodeEditor(typeof(Nodes.PageOptions_External))]
    public class PageOptions_ExternalEditor : DialogNodeEditor {

        public new Nodes.PageOptions_External node => m_node as Nodes.PageOptions_External;

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor.graph is DialogGraph dialog) {
                dialog.options_externals.Add(node.name, node);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor.graph is DialogGraph dialog) {
                dialog.options_externals.Remove(node.name);
            }
        }

        public override void on_body_gui() {
            base.on_body_gui();
            GUILayout.Label(node.name);
        }


        public override bool has_create_wizard => true;

        public override void create_wizard_gui(GraphEditor graph_editor) {
            var node = this.node;
            node.name = EditorGUILayout.TextField("Name", node.name)?.Trim();
            m_create_ok = false;
            if (string.IsNullOrEmpty(node.name)) {

            } else {
                if (graph_editor.graph is DialogGraph dialog) {
                    if (!dialog.options_externals.ContainsKey(node.name)) {
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

        private bool m_create_ok = false;

    }
}
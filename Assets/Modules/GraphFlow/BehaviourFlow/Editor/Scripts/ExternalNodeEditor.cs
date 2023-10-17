
using GraphNode.Editor;
using UnityEngine;
using UnityEditor;

namespace BehaviourFlow.Editor {

    [NodeEditor(typeof(Nodes.ExternalNode))]
    public class ExternalNodeEditor : BTNodeEditor {

        public new Nodes.ExternalNode node => m_node as Nodes.ExternalNode;

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor.graph is BehaviourTree bt) {
                bt.externals.Add(node.name, node);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (view.graph.editor.graph is BehaviourTree bt) {
                bt.externals.Remove(node.name);
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
                if (graph_editor.graph is BehaviourTree bt) {
                    if (!bt.externals.ContainsKey(node.name)) {
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
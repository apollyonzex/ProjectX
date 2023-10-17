
using GraphNode.Editor;
using UnityEditor;
using UnityEngine;

namespace DialogFlow.Editor.NodeEditors {

    [NodeEditor(typeof(Nodes.PageContent))]
    public class PageContentEditor : PageNodeBaseEditor {

        public new Nodes.PageContent node => m_node as Nodes.PageContent;

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            var graph = this.graph.graph;

            if (graph.localizable) {
                var this_node = node;
                if (!string.IsNullOrEmpty(this_node.content.content)) {
                    if (graph.try_localize(this_node.content.content, out var content)) {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.LabelField(content, EditorStyles.wordWrappedLabel);
                        EditorGUILayout.EndVertical();
                    } else {
                        EditorGUILayout.HelpBox("Localize Failed", MessageType.Warning);
                    }
                }
            }
        }
    }
}
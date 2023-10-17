
using UnityEngine;

using GraphNode.Editor;


namespace StateFlow.Editor {

    [NodeEditor(typeof(EntryStateNode))]
    public class EntryStateNodeEditor : StateNodeEditor {

        public override void on_header_gui(GUIStyle style) {
            GUILayout.Label("EntryState", style);
        }
    }
}
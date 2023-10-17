
using UnityEngine;
using GraphNode.Editor;

namespace BehaviourFlow.Editor {

    [NodeEditor(typeof(BTNode))]
    public class BTNodeEditor : GenericNodeEditor {

        public override Color node_color => m_node_color;

        public void _set_node_color(Color color) {
            m_node_color = color;
        }

        public virtual void rebuild_all_expressions() {
            foreach (var pe in m_properties) {
                if (pe is ExpressionEditor ee) {
                    ee.build();
                } else if (pe is ContextActionEditor cae) {
                    cae.build_parameters();
                }
            }
        }


        private Color m_node_color = new Color32(90, 97, 105, 255);

        public float runtime_fading = 0;
        public Color runtime_color;
        public int runtime_updating_index = -1;
        public bool runtime_break_when_pending = false;
        public bool runtime_break_when_success = false;
        public bool runtime_break_when_failed = false;
        public bool runtime_break_when_abort = false;

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            if (view.graph.runtime) {
                GUILayout.BeginVertical(GUI.skin.box);
                runtime_break_when_pending = GUILayout.Toggle(runtime_break_when_pending, "Break When Pending");
                runtime_break_when_success = GUILayout.Toggle(runtime_break_when_success, "Break When Success");
                runtime_break_when_failed = GUILayout.Toggle(runtime_break_when_failed, "Break When Failed");
                runtime_break_when_abort = GUILayout.Toggle(runtime_break_when_abort, "Break When Abort");
                GUILayout.EndVertical();
            }
        }
    }
}
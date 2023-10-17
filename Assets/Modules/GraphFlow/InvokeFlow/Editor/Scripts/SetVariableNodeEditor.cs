
using GraphNode;
using GraphNode.Editor;
using UnityEditor;
using UnityEngine;
using CalcExpr;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(SetVariableNode))]
    public class SetVariableNodeEditor : ExpressionContextNodeWithInputEditor {

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("target", out var vne) && vne is VariableNameEditor target) {
                m_target = target;
            }
            if (try_get_property("expression", out var ee) && ee is ExpressionEditor expr) {
                m_expr = expr;
            }
        }

        protected ExpressionEditor m_expr;
        protected VariableNameEditor m_target;

        public override void on_body_gui() {
            base.on_body_gui();
            bool error;
            if (m_target != null && m_expr != null) {
                if (m_target.val.name == string.Empty) {
                    error = m_expr.err_msg != null;
                } else {
                    error = m_target.target == null || m_expr.err_msg != null || (m_expr.value_type != ValueType.Unknown && m_expr.value_type != (ValueType)m_target.target.type);
                }
            } else {
                error = true;
            }
            if (error) {
                GUILayout.Label("Error", GraphResources.styles.red_label);
            } else if (m_target.val.name != string.Empty) {
                GUILayout.Label($"{m_target.val.name} = ...");
            } else {
                GUILayout.Label("<none>");
            }
        }

        protected override void on_inspector_gui_inner() {
            if (m_target != null && m_expr != null && m_target.target != null) {         
                if (m_expr.err_msg == null && m_expr.value_type != ValueType.Unknown && m_expr.value_type != (ValueType)m_target.target.type) {
                    EditorGUILayout.HelpBox($"Invalid expression type: got {m_expr.value_type}, expected {m_target.target.type}", MessageType.Error);
                }
            }
        }

    }

}
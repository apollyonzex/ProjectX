
using CalcExpr;
using GraphNode;
using GraphNode.Editor;

namespace BehaviourFlow.Editor {

    [PropertyEditor(typeof(ContextAction))]
    public class ContextActionEditor : ActionEditorBase<BTExecutorBase> {

        public override Action<BTExecutorBase> create_action() {
            return new ContextAction();
        }

        public override ExpressionEditorBase create_parameter_editor() {
            return new ExpressionEditor();
        }
    }
}
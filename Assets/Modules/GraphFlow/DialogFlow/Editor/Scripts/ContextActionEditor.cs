

using GraphNode;
using GraphNode.Editor;

namespace DialogFlow.Editor {

    [PropertyEditor(typeof(ContextAction))]
    public class ContextActionEditor : ActionEditorBase {

        public override Action create_action() {
            return new ContextAction();
        }

        public override ExpressionEditorBase create_parameter_editor() {
            return new ExpressionEditor();
        }
    }
}
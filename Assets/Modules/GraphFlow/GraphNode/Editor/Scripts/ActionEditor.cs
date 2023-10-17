
using CalcExpr;

namespace GraphNode.Editor {

    [PropertyEditor(typeof(Action))]
    public class ActionEditor : ActionEditorBase {

        public override Action create_action() {
            return new Action();
        }

        public override ExpressionEditorBase create_parameter_editor() {
            return new ExpressionEditor();
        }

    }

    [PropertyEditor(typeof(Action<>))]
    public class ActionEditor<T> : ActionEditorBase<T> {
        public override Action<T> create_action() {
            return new Action<T>();
        }
        public override ExpressionEditorBase create_parameter_editor() {
            return new ExpressionEditor();
        }
    }
}
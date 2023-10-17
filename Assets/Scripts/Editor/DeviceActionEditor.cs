
using DeviceGraph;
using GraphNode;
using GraphNode.Editor;

namespace Editor {

    [PropertyEditor(typeof(DeviceAction))]
    public class DeviceActionEditor : ActionEditor {
        public override Action create_action() {
            return new DeviceAction();
        }

        public override ExpressionEditorBase create_parameter_editor() {
            return new DeviceExpressionEditor();
        }
    }

    [PropertyEditor(typeof(DeviceAction<>))]
    public class DeviceActionEditor<T> : ActionEditor<T> {
        public override Action<T> create_action() {
            return new DeviceAction<T>();
        }

        public override ExpressionEditorBase create_parameter_editor() {
            return new DeviceExpressionEditor();
        }
    }
}

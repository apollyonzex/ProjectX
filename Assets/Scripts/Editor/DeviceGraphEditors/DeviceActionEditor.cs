
using GraphNode;
using GraphNode.Editor;

namespace World_Formal.BattleSystem.DeviceGraph             //因为demo版本把类名占用了,这个就先放在图流命名空间下
{

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

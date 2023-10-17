using GraphNode;
using GraphNode.Editor;

namespace World_Formal.BattleSystem.DeviceGraph             //因为demo版本把类名占用了,这个就先放在图流命名空间下
{
    [PropertyEditor(typeof(DeviceExpression))]
    public class DeviceExpressionEditor : ExpressionEditor<DeviceExpression>
    {
        public override ExpressionBase create_expression()
        {
            return new DeviceExpression();
        }
    }
}

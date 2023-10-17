using GraphNode;
using GraphNode.Editor;

namespace World_Formal.BattleSystem.DeviceGraph             //��Ϊdemo�汾������ռ����,������ȷ���ͼ�������ռ���
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

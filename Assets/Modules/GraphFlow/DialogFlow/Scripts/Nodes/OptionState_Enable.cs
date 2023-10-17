
using GraphNode;
using UnityEngine;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_Enable : OptionStateNodeBase {

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;


        public override OptionState get_option_state(IContext context) {
            if (condition == null) {
                Debug.LogError("OptionState_Enable: invalid expression");
            } else if (!condition.calc(context, context.context_type, out bool ret)) {
                Debug.LogError("OptionState_Enable: expression failed!");
            } else if (ret) {
                return OptionState.Enable;
            }
            return OptionState.Disable;
        }
    }
}
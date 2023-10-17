
using GraphNode;
using UnityEngine;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_Branch : OptionStateNodeBase {

        [Input]
        [Display("{True}")]
        public OptionStateFunc true_value { get; set; }

        [Input]
        [Display("{False}")]
        public OptionStateFunc false_value { get; set; }


        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        public override OptionState get_option_state(IContext context) {
            if (condition == null) {
                Debug.LogError("OptionState_Branch: invalid expression");
            } else if (!condition.calc(context, context.context_type, out bool ret)) {
                Debug.LogError("OptionState_Branch: expression failed!");
            } else if (ret) {
                return true_value != null ? true_value.Invoke(context) : OptionState.Enable;
            }
            return false_value != null ? false_value.Invoke(context) : OptionState.Enable;
        }
    }

}
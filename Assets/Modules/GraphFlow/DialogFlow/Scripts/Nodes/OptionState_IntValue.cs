
using GraphNode;
using UnityEngine;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_IntValue : OptionStateNodeBase {

        [Input]
        [Display("> 0")]
        public OptionStateFunc value_1 { get; set; }

        [Input]
        [Display("= 0")]
        public OptionStateFunc value_2 { get; set; }

        [Input]
        [Display("< 0")]
        public OptionStateFunc value_3 { get; set; }


        [Display("Expression")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public Expression expression;

        public override OptionState get_option_state(IContext context) {
            if (expression == null) {
                Debug.LogError("OptionState_IntValue: invalid expression");
            } else if (!expression.calc(context, context.context_type, out int ret)) {
                Debug.LogError("OptionState_IntValue: expression failed!");
            } else if (ret > 0) {
                return value_1 != null ? value_1.Invoke(context) : OptionState.Enable;
            } else if (ret < 0) {
                return value_3 != null ? value_3.Invoke(context) : OptionState.Invisible;
            }
            return value_2 != null ? value_2.Invoke(context) : OptionState.Disable;
        }
    }

}

using GraphNode;
using UnityEngine;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_Visible : OptionStateNodeBase {

        [Input]
        [Display("")]
        public OptionStateFunc input { get; set; }

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;


        public override OptionState get_option_state(IContext context) {
            if (condition == null) {
                Debug.LogError("OptionState_Visible: invalid expression");
            } else if (!condition.calc(context, context.context_type, out bool ret)) {
                Debug.LogError("OptionState_Visible: expression failed!");
            } else if (ret) {
                return input != null ? input.Invoke(context) : OptionState.Enable;
            }
            return OptionState.Invisible;
        }
    }
}
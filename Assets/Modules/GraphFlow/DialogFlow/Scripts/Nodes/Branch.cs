
using GraphNode;
using CalcExpr;
using UnityEngine;

namespace DialogFlow {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class Branch : DialogNode {

        [Output]
        [Display("{True}")]
        public DialogAction true_part { get; set; }

        [Output]
        [Display("{False}")]
        public DialogAction false_part { get; set; }

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(ValueType.Boolean)]
        public Expression condition;

        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            if (condition == null) {
                Debug.LogError("Branch: invalid expression");
            } else if (!condition.calc(context, context.context_type, out bool ret)) {
                Debug.LogError("Branch: expression failed!");
            } else if (ret) {
                if (true_part != null) {
                    true_part.Invoke(context);
                    return;
                }
            } else {
                if (false_part != null) {
                    false_part.Invoke(context);
                    return;
                }
            }
            context.end();
        }

    }

}